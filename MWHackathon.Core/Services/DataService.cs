using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace MWHackathonHarvester.Services
{
  public abstract class DataService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(DataService));

    public Feed Feed { get; set; }
    public abstract IEnumerable<Entry> GetEntries();
    public abstract string GetEntryImageUrl(Entry entry);

    /// <summary>
    /// parses OAI XML and returns each item individually
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerable<Entry> GetSubmittableEntries(DatabaseService db)
    {
      List<string> uniqueIds = db.GetUniqueIds(Feed);

      foreach (var entry in GetEntries())
      {
        if (uniqueIds.Contains(entry.object_id))
        {
          log.Debug("Skip " + entry.object_id);
          continue;
        }
        uniqueIds.Add(entry.object_id);
        yield return entry;
      }
    }

  }
}
