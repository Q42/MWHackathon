﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathonHarvester.Models;
using log4net;

namespace MWHackathonHarvester.Services
{
  public class DatabaseService : IDisposable
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseService));

    private DatabaseDataContext db;
    public DatabaseService()
    {
      db = new DatabaseDataContext();
    }

    public List<Feed> GetAllFeeds()
    {
      return db.Feeds.ToList();
    }

    public List<Entry> GetAllEntries(int feedId)
    {
      return db.Entries.Where(e => e.feed_id == feedId).ToList();
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
