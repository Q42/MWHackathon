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
  public class AmsterdamMuseum: XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(AmsterdamMuseum));

    public AmsterdamMuseum(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get
      {
        return "http://amdata.adlibsoft.com/wwwopac.ashx?database=AMcollect&limit=100&search=all&xmltype=grouped";
      }
    }

    private int Total = 21;
    private int ResultsPerPage = 100;
    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      XmlDocument doc = Utils.DownloadXml(Url);
      Total = Convert.ToInt32(doc.SelectSingleNode("/adlibXML/diagnostic/hits").InnerText);
      yield return doc;

      int page = 1;
      while (page * ResultsPerPage < Total)
      {
        doc = Utils.DownloadXml(Url + "&startfrom=" + ((page * ResultsPerPage)+1));
        yield return doc;
        page++;
      }
    }

    public override string XPathToRecord
    {
      get { return "/adlibXML/recordList/record"; }
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
      XmlElement img = el.SelectSingleNode("reproduction/reproduction.identifier_URL") as XmlElement;
      if (img != null && img.InnerText.Length > 28)
        return string.Format("http://ahm.adlibsoft.com/ahmimages/{0}", img.InnerText.ToLowerInvariant().Replace(@"..\..\dat\collectie\images\","").Replace("\\","/"));
      return null;
    }

    public override string GetEntryImageUrl(Entry entry)
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(entry.body);
      return GetEntryImageUrl(doc.DocumentElement);
    }
  }
}