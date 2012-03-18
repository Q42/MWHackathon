using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MWHackathonHarvester.Services;
using System.Configuration;
using System.Xml;
using MWHackathon.Core.Models;

namespace MWHackathonHarvester.Feeds
{
  public class VictoriaAndAlbertMuseum: XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(VictoriaAndAlbertMuseum));

    public VictoriaAndAlbertMuseum(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get { return "http://www.vam.ac.uk/api/xml/museumobject?limit=45"; }
    }

    private int AmountPerPage = 45;

    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      int amount = AmountPerPage;
      int page = 0;
      while (amount == AmountPerPage)
      {
        XmlDocument doc = Utils.DownloadXml(Url + "&offset=" + page * AmountPerPage);
        amount = doc.SelectNodes(XPathToRecord).Count;
        yield return doc;
        page++;
      }
    }

    public override string XPathToRecord
    {
      get { return "/varesponse/museumobject"; }
    }

    public override string GetEntryId(XmlElement el)
    {
      return GetXpathInnerText(el, "object_number");
    }

    public override string GetEntryName(XmlElement el)
    {
      return GetXpathInnerText(el, "title");
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      string imageId = GetXpathInnerText(el, "primary_image_id");
      string prefix = "http://media.vam.ac.uk/media/thira/collection_images";
      
      if (!string.IsNullOrEmpty(imageId))
        return prefix + "/" + imageId.Substring(0, 6) + "/" + imageId + ".jpg";
      return null;
    }
  }
}
