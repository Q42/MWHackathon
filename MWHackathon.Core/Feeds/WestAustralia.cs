using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using MWHackathonHarvester.Services;
using System.Configuration;
using System.Xml;

namespace MWHackathonHarvester.Feeds
{
  public class WestAustralia: XMLService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(WestAustralia));

    public WestAustralia(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get { return "http://www.museum.wa.gov.au/maritime-archaeology-db/rest/node/"; }
    }
    private int Total;
    private int AmountPerPage = 20;

    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      int page = 0;
      while (page < 2360)
      {
        yield return Utils.DownloadXml(Url + "?page=" + page);
        page++;
      }
    }

    public override string XPathToRecord
    {
      get { return "/result/item"; }
    }

    public override string GetEntryId(XmlElement el)
    {
      return GetXpathInnerText(el, "nid");
    }

    public override string GetEntryName(XmlElement el)
    {
      return GetXpathInnerText(el, "title");
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      return null;
    }
  }
}
