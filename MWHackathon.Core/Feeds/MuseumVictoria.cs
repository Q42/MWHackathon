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
  public class MuseumVictoria: XMLService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(MuseumVictoria));

    public MuseumVictoria(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get { return "http://museumvictoria.com.au/collections/api/v1/items/search?size=500&q=photograph"; }
    }
    private int Total;
    private int AmountPerPage = 500;

    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      XmlDocument doc = Utils.DownloadXml(Url);
      Total = Convert.ToInt32(doc.SelectSingleNode("/response/result/items/totalItems").InnerText);
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
      get { return "/response/result/items/pagedItems/item"; }
    }

    public override string GetEntryId(XmlElement el)
    {
      return GetXpathInnerText(el, "id");
    }

    public override string GetEntryName(XmlElement el)
    {
      return GetXpathInnerText(el, "name");
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      string imgid = GetXpathInnerText(el, "image/id");
      if (string.IsNullOrEmpty(imgid))
        return imgid;
      imgid = imgid.PadLeft(6, '0');
      string url = string.Format("http://museumvictoria.com.au/collections/itemimages/{0}/{1}/{2}_{3}.jpg",
        imgid.Substring(0,3),
        imgid.Substring(3,3),
        imgid,
        "medium"
        );
      return url;
    }
  }
}
