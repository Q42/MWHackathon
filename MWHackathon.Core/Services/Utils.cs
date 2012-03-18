using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;

namespace MWHackathonHarvester.Services
{
  public static class Utils
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(Utils));

    public static XmlDocument DownloadXml(string url)
    {
      log.DebugFormat("Downloading {0}", url);

      XmlDocument doc = new XmlDocument();
      doc.Load(url);
      return doc;
    }

  }
}
