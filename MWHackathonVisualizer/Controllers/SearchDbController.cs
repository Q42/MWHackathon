using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWHackathonHarvester.Services;
using MWHackathon.Core.Models;
using System.Data.Linq;

namespace MWHackathonVisualizer.Controllers
{
  public class SearchDbController : Controller
  {
    public ActionResult Index()
    {
      using (var db = new DatabaseService())
      {
        ViewBag.Feeds = db.GetAllFeeds().ToList();
        ViewBag.tiled = false;
        ViewBag.faces = false;

        IEnumerable<KeyValuePair<Feed, int>> feeds = db.GetAllFeeds().Select(f => new KeyValuePair<Feed, int>(f, f.Entries.Count()));
        return View(feeds.ToList());
      }
    }

    public ActionResult Search(int feedId, string query, int? tiled, int? faces)
    {
      bool tiledValue = tiled.HasValue && tiled.Value == 1;
      bool facesValue = faces.HasValue && faces.Value == 1;
      using (var db = new DatabaseService())
      {
        ViewBag.Feeds = db.GetAllFeeds().ToList();
        ViewBag.tiled = tiledValue;
        ViewBag.faces = facesValue;

        var entries = db.GetAllEntries();
        if (feedId > 0)
          entries = entries.Where(e => e.feed_id == feedId);
        if (tiledValue)
          entries = entries.Where(e => e.imagewidth != null && e.imageheight != null);
        if (facesValue)
          entries = entries.Where(e => e.facial_amount > 0).OrderByDescending(e => e.facial_amount);

        if (!string.IsNullOrEmpty(query))
        {
          int idQuery = 0;
          Int32.TryParse(query, out idQuery);
          entries = entries.Where(e => e.id == idQuery || e.object_name.Contains(query));
        }
        ViewBag.Amount = entries.Count();

        if (tiledValue)
          return View("tiled", entries.Take(500).ToList());
        return View(entries.Take(100).ToList());
      }
    }

  }
}
