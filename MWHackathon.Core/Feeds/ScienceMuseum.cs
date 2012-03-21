using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using MWHackathonHarvester.Services;
using System.Configuration;
using System.Xml;
using System.IO;

namespace MWHackathonHarvester.Feeds
{
  public class ScienceMuseum: CSVService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(ScienceMuseum));
    private Dictionary<string, string> ObjectMediaMapping;

    public ScienceMuseum(Feed feed)
    {
      this.Feed = feed;
      FillObjectMediaMapping();
    }

    private void FillObjectMediaMapping()
    {
      var file = new FileInfo(@"c:\inetpub\wwwroot\MWHackathon\Data\NMSI_media_20110304.csv");
      var parsed = Utils.ParseCSV(file, separator, firstLineContainsHeaders);

      ObjectMediaMapping = new Dictionary<string, string>();
      foreach (var row in parsed)
      {
        if (!ObjectMediaMapping.ContainsKey(row.Value.ElementAt(1)))
          ObjectMediaMapping.Add(row.Value.ElementAt(1), row.Value.ElementAt(2));
      }
      log.Info("Loaded objectmediamapping: " + ObjectMediaMapping.Count);
    }

    public override IEnumerable<FileInfo> Files
    {
      get {
        yield return new FileInfo(@"c:\inetpub\wwwroot\MWHackathon\Data\NMSI_object1_20110304.csv");
        yield return new FileInfo(@"c:\inetpub\wwwroot\MWHackathon\Data\NMSI_object2_20110304.csv");
        yield return new FileInfo(@"c:\inetpub\wwwroot\MWHackathon\Data\NMSI_object3_20110304.csv");
        yield return new FileInfo(@"c:\inetpub\wwwroot\MWHackathon\Data\NMSI_object4_20110304.csv");
      }
    }

    public override string separator
    {
      get { return ","; }
    }

    public override bool firstLineContainsHeaders
    {
      get { return true; }
    }

    public override string GetEntryId(List<string> el)
    {
      return el.ElementAt(0);
    }

    public override string GetEntryName(List<string> el)
    {
      return el.ElementAt(2);
    }

    public override string GetEntryImageUrl(List<string> el)
    {
      string id = GetEntryId(el);
      if (ObjectMediaMapping.ContainsKey(id))
        return string.Format("http://collectionsonline.nmsi.ac.uk/grabimg.php?wm=1&kv={0}", ObjectMediaMapping[id]);
      return null;
    }
  }
}
