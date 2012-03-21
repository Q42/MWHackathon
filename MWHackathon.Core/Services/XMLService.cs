using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;

namespace MWHackathonHarvester.Services
{
  public abstract class XMLService : DataService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(XMLService));

    public abstract IEnumerable<XmlDocument> GetPagedXml();
    public abstract string XPathToRecord { get; }
    public abstract string GetEntryId(XmlElement el);
    public abstract string GetEntryName(XmlElement el);
    public abstract string GetEntryImageUrl(XmlElement el);

    /// <summary>
    /// parses OAI XML and returns each item individually
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public override IEnumerable<Entry> GetEntries()
    {
      DateTime downloadedDate = DateTime.Now;
      foreach (var xml in GetPagedXml())
      {
        foreach (XmlElement el in xml.SelectNodes(XPathToRecord))
        {
          string id = GetEntryId(el);

          if (string.IsNullOrEmpty(id))
          {
            log.Error("No ID found for element, skipping...");
            continue;
          }

          string name = GetEntryName(el);
          string imageurl = GetEntryImageUrl(el);

          yield return new Entry
          {
            body = el.OuterXml,
            feed_id = Feed.id,
            lastimported_date = downloadedDate,
            object_id = id,
            object_name = name,
            object_imageurl = imageurl
          };
        }
      }
    }

    public string GetXpathInnerText(XmlElement el, string xpath)
    {
      string id = null;
      XmlNode idEl = el.SelectSingleNode(xpath);
      if (idEl != null)
        id = idEl.InnerText;
      return id;

    }

  }
}
