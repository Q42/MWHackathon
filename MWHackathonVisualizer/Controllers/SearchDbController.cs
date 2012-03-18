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

        IEnumerable<KeyValuePair<Feed, int>> feeds = db.GetAllFeeds().Select(f => new KeyValuePair<Feed, int>(f, f.Entries.Count()));
        return View(feeds.ToList());
      }
    }

    public ActionResult Search(int feedId, string query)
    {
      using (var db = new DatabaseService())
      {
        ViewBag.Feeds = db.GetAllFeeds().ToList();

        var entries = db.GetAllEntries();
        if (feedId > 0)
          entries = entries.Where(e => e.feed_id == feedId);
        if (!string.IsNullOrEmpty(query))
        {
          entries = entries.Where(e => e.object_name.Contains(query));
        }
        return View(entries.Take(100).ToList());
      }
    }

  }
}
