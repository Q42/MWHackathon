using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace MWHackathonHarvester.Services
{
  public abstract class CSVService : DataService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(CSVService));

    public abstract IEnumerable<FileInfo> Files { get; }
    public abstract string separator { get; }
    public abstract bool firstLineContainsHeaders { get; }
    public abstract string GetEntryId(List<string> el);
    public abstract string GetEntryName(List<string> el);
    public abstract string GetEntryImageUrl(List<string> el);

    public override string GetEntryImageUrl(Entry entry)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// parses OAI XML and returns each item individually
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public override IEnumerable<Entry> GetEntries()
    {
      DateTime downloadedDate = DateTime.Now;
      foreach (var File in Files)
      {
        foreach (var row in Utils.ParseCSV(File, separator, firstLineContainsHeaders))
        {
          if (row.Value.Count < 3)
          {
            log.Error("Less than 3 columns found, skipping...");
            continue;
          }

          string id = GetEntryId(row.Value);

          if (string.IsNullOrEmpty(id))
          {
            log.Error("No ID found for element, skipping...");
            continue;
          }

          string name = GetEntryName(row.Value);
          string imageurl = GetEntryImageUrl(row.Value);

          yield return new Entry
          {
            body = row.Key,
            feed_id = Feed.id,
            lastimported_date = downloadedDate,
            object_id = id,
            object_name = name,
            object_imageurl = imageurl
          };
        }
      }
    }




  }
}
