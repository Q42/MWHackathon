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
  public class StedelijkMuseum: XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(StedelijkMuseum));

    public StedelijkMuseum(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get
      {
        return "http://stedelijkapi.q42.net/xml?perpage=100";
      }
    }

    private int ResultsPerPage = 100;
    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      int perPage = ResultsPerPage;
      int page = 0;
      while (perPage == ResultsPerPage)
      {
        page++;
        var doc = Utils.DownloadXml(Url + "&page=" + page);
        yield return doc;
        perPage = doc.SelectNodes(XPathToRecord).Count;
      }
    }

    public override string XPathToRecord
    {
      get { return "/records/record"; }
    }

    public override string GetEntryId(XmlElement el)
    {
      return el.GetAttribute("priref");
    }

    public override string GetEntryName(XmlElement el)
    {
      return GetXpathInnerText(el, "title");
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      XmlElement img = el.SelectSingleNode("reproduction.identifier_URL") as XmlElement;
      if (img != null && img.InnerText.Length > 13)
        return string.Format("http://stedelijkassets.q42.net/adlib/lightbox/{0}", img.InnerText.Replace(@"../images/", ""));
      return null;
    }
  }
}