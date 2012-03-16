using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathonHarvester.Models;
using log4net;

namespace MWHackathonHarvester.Services
{
  public class DatabaseService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseService));

    public List<Feed> GetAllFeeds()
    {
      using (var db = new DatabaseDataContext())
        return db.Feeds.ToList();
    }

    public List<Entry> GetAllEntries(int FeedId)
    {
      using (var db = new DatabaseDataContext())
        return db.Entries.Where(e => e.feed_id == FeedId).ToList();
    }

    public Feed GetFeed(string name)
    {
      using (var db = new DatabaseDataContext())
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
    }


  }
}
