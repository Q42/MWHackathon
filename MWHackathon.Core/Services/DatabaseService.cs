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
      int count = 0;
      int skipped = 0;
      foreach (var entry in entries)
      {
        count++;

        if (count > 100)
          return;

        // check if exists
        Entry existing = GetEntry(entry.object_id, entry.feed_id);
        if (existing != null)
        {
          skipped++;
          continue;
        }

        if (skipped > 0)
        {
          log.Debug("Skipped " + skipped + " entries");
          skipped = 0;
        }

        log.Debug("Inserting " + entry.object_id);

        db.Entries.InsertOnSubmit(entry);
        db.SubmitChanges();
      }
    }

    public void Dispose()
    {
      db.Dispose();
    }
  }
}
