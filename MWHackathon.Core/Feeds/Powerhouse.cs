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
  public class Powerhouse: XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(Powerhouse));

    public Powerhouse(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get { return "http://api.powerhousemuseum.com/api/v1/item/xml/?api_key=2625735b3217b94"; }
    }
    private int Total;
    private int AmountPerPage = 50;

    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      XmlDocument doc = Utils.DownloadXml(Url);
      Total = Convert.ToInt32(doc.SelectSingleNode("/result/total").InnerText);
      yield return doc;

      int page = 1;
      while (page * AmountPerPage < Total)
      {
        doc = Utils.DownloadXml(Url + "&start=" + (page * AmountPerPage));
        yield return doc;
        page++;
      }
    }

    public override string XPathToRecord
    {
      get { return "/result/items/item"; }
    }

    public override string GetEntryId(XmlElement el)
    {
      return GetXpathInnerText(el, "id");
    }

    public override string GetEntryName(XmlElement el)
    {
      string title = GetXpathInnerText(el, "title");
      if (string.IsNullOrEmpty(title) || title == "None")
        return GetXpathInnerText(el, "summary");
      return title;
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      string url = GetXpathInnerText(el, "multimedia_uri");
      if (!string.IsNullOrEmpty(url))
      {
        var xml = Utils.DownloadXml("http://api.powerhousemuseum.com" + url);
        return GetXpathInnerText(xml.DocumentElement, "/result/multimedia/multimedia/images/*/url[.!=''][1]");
      }
      return null;
    }
  }
}
