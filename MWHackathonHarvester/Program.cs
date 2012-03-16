using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MWHackathonHarvester.Services;

namespace MWHackathonHarvester
{
  class Program
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(Program));

    static void Main(string[] args)
    {
      log4net.Config.XmlConfigurator.Configure();
      log.Info("Starting harvest");

      var db = new DatabaseService();

      var feed = db.GetFeed("Rijksmuseum Amsterdam");

      Console.WriteLine("Done!");
      Console.ReadKey(true);
    }
  }
}
