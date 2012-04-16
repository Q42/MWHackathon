using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWHackathonHarvester.Services;
using MWHackathon.Core.Models;
using System.Data.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace MWHackathonVisualizer.Controllers
{
  public class FacialController : Controller
  {
    public ActionResult Index()
    {
      var rnd = new Random();
      using (var db = new DatabaseService())
      {
        var allentries = GetEntries(db, 1);
        var dbNextEntry = allentries.ElementAt(rnd.Next(0, allentries.Count));
        var dbNextFace = GetFace(dbNextEntry);

        return View(dbNextEntry.id);
      }
    }

    public ActionResult Overlay(string url, int entry)
    {
      if (string.IsNullOrEmpty(url))
        url = "https://twimg0-a.akamaihd.net/profile_images/769005756/paulstork_foto.png";
        //url = "http://museummobile.info/wp-content/uploads/2010/05/NancyPic.png";

      using (var db = new DatabaseService())
      {
        string cachefile = @"c:\MWHackathon\Assets\uploaded\" + url.Substring(url.LastIndexOf('/') + 1) + ".json";
        string json = "";
        if (System.IO.File.Exists(cachefile))
          json = System.IO.File.ReadAllText(cachefile);
        else
        {
          json = db.FacialData(url);
          System.IO.File.WriteAllText(cachefile, json);
        }
        var upFaces = new Faces(json, url, null, null);

        int amount = upFaces.Amount;
        if (amount < 1)
        {
          Response.Write("Darn, I can't find a face in there! Please go back and try a different one.");
          return new EmptyResult();
        }
        if (amount > 1)
        {
          Response.Write("Sorry, this one seems to have multiple faces. I haven't built that yet. Please go back and try a different one.");
          return new EmptyResult();
        }

        var dbEntry = db.GetAllEntries().First(e => e.id == entry);
        var dbFace = GetFace(dbEntry);
        var rnd = new Random();
        var allentries = GetEntries(db, amount);
        var dbNextEntry = allentries.ElementAt(rnd.Next(0,allentries.Count));
        var dbNextFace = GetFace(dbNextEntry);

        ViewBag.Entry = dbEntry;
        ViewBag.Faces = upFaces;
        ViewBag.NextFace = dbNextEntry;
        return View(dbFace);
      }

    }

    private List<Entry> GetEntries(DatabaseService db, int amountOfFaces)
    {
      List<int> preferredids = new List<int>() {
          2468, 48083, 47949, 23679, 41523, 15819, 48189, 38872, 23689, 23685, 38462,
          3088, 23683, 40049, 4799, 39316, 40495, 20599, 18432, 40497
        };

      List<int> excludeids = new List<int>() {
          21080, 13728, 16923, 24345, 38270, 14921, 24373
        };


      var rnd = new Random();
      var dbEntries = db.GetAllEntries().Where(e => e.facial_amount == amountOfFaces && (e.imageheight >= 350 || e.imagewidth >= 350));
      dbEntries = dbEntries.Where(e => e.feed_id != 1); // not rijksmuseum, size klopt niet!
      dbEntries = dbEntries.Where(e => preferredids.Contains(e.id));
      dbEntries = dbEntries.Where(e => !excludeids.Contains(e.id));
      return dbEntries.OrderBy(e => rnd.Next()).Take(100).ToList();

    }


    private List<Faces> GetFaces(List<Entry> entries)
    {
      var result = new List<Faces>();

      foreach (var entry in entries)
        result.Add(GetFace(entry));

      return result;
    }

    private Faces GetFace(Entry entry)
    {
      return new Faces(entry.facialdata, entry.object_imageurl, entry.imagewidth, entry.imageheight);
    }
  }
}
