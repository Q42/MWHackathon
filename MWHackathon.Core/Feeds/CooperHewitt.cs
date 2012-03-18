using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using MWHackathonHarvester.Services;
using System.Configuration;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MWHackathonHarvester.Feeds
{
  public class CooperHewitt: JSONService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(CooperHewitt));

    public CooperHewitt(Feed feed)
    {
      this.Feed = feed;
    }


    public override IEnumerable<JObject> GetObjects()
    {
      // get from disk
      DirectoryInfo dir = new DirectoryInfo(@"c:\inetpub\collection\objects");
      if (!dir.Exists) throw new DirectoryNotFoundException("Can't find CooperHewitt's files, download them from https://github.com/cooperhewitt/collection.git!");
      var files = dir.GetFiles("*.json", SearchOption.AllDirectories);
      log.DebugFormat("Found {0} json files in {1}", files.Length, dir.FullName);
      foreach (var file in files)
      {
        yield return JObject.Parse(File.ReadAllText(file.FullName));
      }
    }

    public override string GetEntryId(JObject obj)
    {
      return GetText(obj, "id");
    }

    public override string GetEntryName(JObject obj)
    {
      return GetText(obj, "title");
    }

    public override string GetEntryImageUrl(JObject obj)
    {
      return GetText(obj, "thumbnail");
    }
  }
}
