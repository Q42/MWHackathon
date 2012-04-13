using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace MWHackathonHarvester.Services
{
  public class DatabaseService : IDisposable
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseService));

    private DatabaseDataContext db;
    public DatabaseService()
    {
      db = new DatabaseDataContext(ConfigurationManager.ConnectionStrings["MWHackathon3LLaptop"].ConnectionString);
    }

    public void DeleteEverything()
    {
      //db.ExecuteCommand("delete from entries");
      //db.ExecuteCommand("delete from feeds");
    }

    public IQueryable<Feed> GetAllFeeds()
    {
      return db.Feeds;
    }

    public IQueryable<Entry> GetAllEntries(int feedId)
    {
      return db.Entries.Where(e => e.feed_id == feedId);
    }

    public IQueryable<Entry> GetAllEntries()
    {
      return db.Entries;
    }

    public Feed GetFeed(string name)
    {
      var feed = db.Feeds.SingleOrDefault(f => f.name == name);
      if (feed == null)
      {
        log.Warn("Creating new feed: " + name);
        feed = new Feed() { name = name };
        db.Feeds.InsertOnSubmit(feed);
        db.SubmitChanges();
      }
      return feed;
    }

    public Entry GetEntry(string objectId, int feedId)
    {
      return db.Entries.SingleOrDefault(e => e.object_id == objectId && e.feed_id == feedId);
    }

    public void UpdateEntries(IEnumerable<Entry> entries, DataService service, ImageService img)
    {
      try
      {

        foreach (var entry in entries)
        {
          try
          {
            var oldImage = entry.object_imageurl;
            var newImage = service.GetEntryImageUrl(entry);

            if (!string.IsNullOrEmpty(newImage) && (string.IsNullOrEmpty(oldImage) || !oldImage.Equals(newImage, StringComparison.InvariantCultureIgnoreCase)))
            {
              entry.object_imageurl = newImage;
              db.SubmitChanges();

              DownloadImage(entry, img);
              log.Debug("Updating and downloading " + entry.object_name);
            }
          }
          catch (Exception ex)
          {
            log.Fatal("Error updating entry: " + entry.object_id + ", " + entry.object_name + ", " + entry.object_imageurl + ", " + entry.feed_id, ex);
          }
        }

      }
      catch (Exception ex)
      {
        log.Fatal("Error, stopping feed!", ex);
      }
    }

    public void SaveEntries(IEnumerable<Entry> entries)
    {
      try
      {
        int countErrored = 0;
        int count = 0;
        int skipped = 0;
        foreach (var entry in entries)
        {
          count++;

          //if (count > 100)
          //  return;

          // check if exists
          //Entry existing = GetEntry(entry.object_id, entry.feed_id);
          //if (existing != null)
          //{
          //  skipped++;
          //  continue;
          //}

          //if (skipped > 0)
          //{
          //  log.Debug("Skipped " + skipped + " entries");
          //  skipped = 0;
          //}

          if (entry.object_imageurl != null && entry.object_imageurl.Length > 255)
          {
            log.Warn("ObjectImageUrl too long! " + entry.object_imageurl);
            continue;
          }
          if (entry.object_name != null && entry.object_name.Length > 255)
          {
            log.Warn("ObjectName too long! " + entry.object_name);
            continue;
          }

          try
          {
            db.Entries.InsertOnSubmit(entry);
            db.SubmitChanges();
            log.Debug("Inserting " + entry.object_id);
          }
          catch (Exception ex)
          {
            skipped++;
            countErrored++;
            if (countErrored > 10)
              throw;
            else
              log.Fatal("Error nr " + countErrored + ", entry: " + entry.object_id + ", " + entry.object_name + ", " + entry.object_imageurl + ", " + entry.feed_id, ex);
          }
        }
      }
      catch (Exception ex)
      {
        log.Fatal("Error, stopping feed!", ex);
      }
    }

    public void Dispose()
    {
      db.Dispose();
    }

    public List<string> GetUniqueIds(Feed Feed)
    {
      return db.Entries.Where(e => e.feed_id == Feed.id).Select(e => e.object_id).ToList();
    }

    public void DownloadImage(Entry entry, ImageService img)
    {
      var file = img.GetFilePath(entry);
      if (file != null && !file.Exists)
      {
        try
        {
          img.DownloadImage(entry.object_imageurl, file);
          Console.Write(".");
        }
        catch (FileNotFoundException)
        {
          log.Info("Image not found: " + entry.object_imageurl);
        }
      }
    }

    public void UploadVoorKars(List<int> entries, ImageService img, bool museumobject)
    {
      var feeds = db.Feeds.ToList();
      int amount = entries.Count();
      int count = 0;
      foreach (var entryid in entries)
      {
        count++;

        Entry entry = db.Entries.FirstOrDefault(e => e.id == entryid);
        if (museumobject && !entry.body.ToLowerInvariant().Contains("photograph"))
        {
          log.Info("skip! " + count + "/" + amount);
          continue;
        }

        using (WebClient client = new WebClient())
        {
          string url = "http://museumornot.appspot.com/api/addartobject";
          //url += "?title=" + Uri.EscapeUriString(entry.object_name);
          //url += "&description=" + Uri.EscapeUriString(entry.body);
          //url += "&url=";
          //url += "&image_url=" + Uri.EscapeUriString(entry.body);
          //url += "&width=" + (entry.imagewidth.HasValue ? entry.imagewidth.ToString() : "0");
          //url += "&height=" + (entry.imageheight.HasValue ? entry.imageheight.ToString() : "0");
          //url += "&institution=" + Uri.EscapeUriString(feeds.Single(f => f.id == entry.feed_id).name);
          var nvc = new NameValueCollection();
          nvc.Add("title", entry.object_name);
          nvc.Add("in_a_museum", museumobject ? "true" : "false");
          nvc.Add("description", entry.body);
          nvc.Add("image_url", entry.object_imageurl);
          nvc.Add("width", (entry.imagewidth.HasValue ? entry.imagewidth.ToString() : "0"));
          nvc.Add("height",  (entry.imageheight.HasValue ? entry.imageheight.ToString() : "0"));
          nvc.Add("institution", feeds.Single(f => f.id == entry.feed_id).name);
          var result = client.UploadValues(url, nvc);
          log.Info("sent " + result + ", progress: " + count + "/" + amount);
        }
      }
    }

    public void ImageSizes(List<int> entries, ImageService img)
    {
      int amount = entries.Count();
      int count = 0;
        foreach (var entryid in entries)
        {
          count++;
          Entry entry = db.Entries.FirstOrDefault(e => e.id == entryid);
          //if (!entry.body.ToLowerInvariant().Contains("photograph"))
          //{
            //log.Info("skip! " + count + "/" + amount);
            //continue;
          //}
          var file = img.GetFilePath(entry);
          if (file != null && file.Exists)
          {
            //log.Info("loading " + file.FullName);
            try
            {
              using (var bmp = Bitmap.FromFile(file.FullName))
              {

                //dbEntry.imagewidth = bmp.Width;
                //dbEntry.imageheight = bmp.Height;
                db.ExecuteCommand("update entries set imagewidth=" + bmp.Width + ", imageheight = " + bmp.Height + " where id = " + entry.id);

                //db.SubmitChanges();
                log.Info("size of " + file.Name + " = " + bmp.Width + "x" + bmp.Height + " , " + count + "/" + amount);
              }
            }
            catch (Exception ex)
            {
              log.Warn("Error loading " + file.FullName);
            }
          }
          else
          {
            log.Info(count + "/" + amount);
          }

      }
    }

    public IQueryable<Entry> GetAllEntriesWithImages()
    {
      return GetAllEntries().Where(e => e.imagewidth != null && e.imagewidth > 0);
    }

    public void RecognizeFaces(List<int> ids)
    {
      int amount = ids.Count;
      int count = 0;
      int per = 0;
      foreach (var id in ids)
      {
        count++;

        int newper = percentage(count, 5000);
        if (per != newper)
        {
          log.Info(newper + "%");
          per = newper;
        }


        if (count > 4950)
        {
          log.Warn("api limit reached :(");
          break;
        }
        Entry entry = db.Entries.Single(e => e.id == id);

        entry.facialdata = FacialData(entry.object_imageurl);
        db.SubmitChanges();
      }
    }

    public string FacialData(string imageurl)
    {
      string url = "http://api.face.com/faces/detect.format?api_key=b9eaef84172552688d4f26d81e9238cf&api_secret=c3b7da3da7ff7228f6980397ffb26cbc&urls=" + imageurl;
      string json = null;
      using (WebClient client = new WebClient())
      {
        json = client.DownloadString(url);
      }
      return json;
    }

    private static int percentage(int done, int max)
    {
      return Convert.ToInt32(((double)done / (double)max) * 100);
    }


    public void FacesAmount(List<int> ids)
    {
      int amount = ids.Count;
      int count = 0;
      int per = 0;
      foreach (var id in ids)
      {
        count++;

        int newper = percentage(count, 5000);
        if (per != newper)
        {
          log.Info(newper + "%");
          per = newper;
        }

        var entry = db.Entries.Single(e => e.id == id);
        
        JObject obj = JObject.Parse(entry.facialdata);
        var photos = obj.SelectToken("photos")[0];
        var faces = photos.SelectToken("tags");
        if (faces != null)
        {
          int amountfaces = faces.Count();
          if (amountfaces == 1 && entry.facialdata.IndexOf("eye_left") != entry.facialdata.LastIndexOf("eye_left"))
            throw new Exception("I expect more faces!");
          entry.facial_amount = faces.Count();
          db.SubmitChanges();
        }
      }
    }
  }
}
