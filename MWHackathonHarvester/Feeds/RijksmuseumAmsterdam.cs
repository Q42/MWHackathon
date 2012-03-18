using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathonHarvester.Models;
using log4net;
using MWHackathonHarvester.Services;
using System.Configuration;
using System.Xml;

namespace MWHackathonHarvester.Feeds
{
  public class RijksmuseumAmsterdam : XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(RijksmuseumAmsterdam));

    public RijksmuseumAmsterdam(Feed feed)
    {
      this.Feed = feed;
    }

    private string Url
    {
      get { return "http://www.rijksmuseum.nl/api/oai/19d6f432-1576-4811-9dff-60d7e9d864c1/?verb=listrecords&metadataPrefix=oai_dc"; }
    }
    private string ResumptionToken;
    public override string XPathToRecord
    {
      get { return "//*[name()='record']"; }
    }

    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      XmlDocument doc = Utils.DownloadXml(Url);
      ResumptionToken = doc.SelectSingleNode("/*/*/*[name()='resumptionToken']").InnerText;
      yield return doc;

      // next pages
      while (!string.IsNullOrEmpty(ResumptionToken))
      {
        doc = Utils.DownloadXml(Url + "&resumptiontoken=" + ResumptionToken);
        ResumptionToken = null;
        ResumptionToken = doc.SelectSingleNode("/*/*/*[name()='resumptionToken']").InnerText;
        yield return doc;
      }
    }

    public override string GetEntryId(XmlElement el)
    {
      return GetXpathInnerText(el, ".//*[name()='dc:identifier']");
    }

    public override string GetEntryName(XmlElement el)
    {
      return GetXpathInnerText(el, ".//*[name()='dc:title']");
    }
  }
}
