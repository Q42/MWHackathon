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
  public class BrooklynMuseum: XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(BrooklynMuseum));

    public BrooklynMuseum(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get
      {
        return "http://www.brooklynmuseum.org/opencollection/api/?method=collection.search&api_key=FqLANkHkqv&item_type=object"
          + "&format=xml&version=1&max_thumb_size=192&date_range_begin=-90000&date_range_end=" + DateTime.Now.AddYears(1).Year;
      }
    }

    private int Total = 21;
    private int ResultsPerPage = 20;
    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      int page = 0;
      while (page * ResultsPerPage < Total)
      {
        XmlDocument doc = Utils.DownloadXml(Url + "&start_index=" + (page * ResultsPerPage));
        Total = Convert.ToInt32( doc.SelectSingleNode("/response/resultset/@total").Value );
        yield return doc;
        page++;
      }
    }

    public override string XPathToRecord
    {
      get { return "/response/resultset/items/object"; }
    }

    public override string GetEntryId(XmlElement el)
    {
      return el.GetAttribute("id");
    }

    public override string GetEntryName(XmlElement el)
    {
      return el.GetAttribute("title");
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      XmlElement img = el.SelectSingleNode("images/image[1]") as XmlElement;
      if (img != null)
        return img.GetAttribute("uri");
      return null;
    }
  }
}
