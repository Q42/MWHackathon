using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathonHarvester.Models;
using log4net;
using System.Xml;

namespace MWHackathonHarvester.Services
{
  public abstract class OAIService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(OAIService));

    public abstract Feed Feed { get; }
    public abstract IEnumerable<XmlDocument> GetPagedXml();

    /// <summary>
    /// parses OAI XML and returns each item individually
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerable<Entry> GetEntries()
    {
      DateTime downloadedDate = DateTime.Now;
      foreach (var xml in GetPagedXml())
      {
        foreach (XmlElement el in xml.SelectNodes("//*[name()='record']"))
        {
          string id = null;
          XmlNode idEl = el.SelectSingleNode(".//*[name()='dc:identifier']");
          if (idEl != null)
            id = idEl.InnerText;

          string name = null;
          XmlNode nameEl = el.SelectSingleNode(".//*[name()='dc:title']");
          if (nameEl != null)
            name = nameEl.InnerText;


          if (string.IsNullOrEmpty(id))
          {
            log.Error("No ID found for element, skipping...");
            continue;
          }

          yield return new Entry
          {
            body = el.OuterXml,
            feed_id = Feed.id,
            lastimported_date = downloadedDate,
            object_id = id,
            object_name = name
          };
        }
      }
    }

  }
}
