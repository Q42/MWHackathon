using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Configuration;

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
      db.ExecuteCommand("delete from entries");
      db.ExecuteCommand("delete from feeds");
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
            //return;

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
            //countErrored++;
            //if (countErrored > 10)
            //  throw;
            //else
            //  log.Fatal("Error nr " + countErrored + ", entry: " + entry.object_id + ", " + entry.object_name + ", " + entry.object_imageurl + ", " + entry.feed_id, ex);
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
  }
}
