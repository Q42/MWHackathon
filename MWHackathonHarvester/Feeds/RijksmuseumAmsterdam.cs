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
  public class RijksmuseumAmsterdam : OAIService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(RijksmuseumAmsterdam));
    private readonly Feed feed;

    public RijksmuseumAmsterdam(Feed feed)
    {
      this.feed = feed;
    }

    private string Url
    {
      get { return "http://www.rijksmuseum.nl/api/oai/19d6f432-1576-4811-9dff-60d7e9d864c1/?verb=listrecords&metadataPrefix=oai_dc"; }
    }
    private string ResumptionToken
    {
      get;
      set;
    } 

    public override Feed Feed
    {
      get { return feed; }
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
  }
}
