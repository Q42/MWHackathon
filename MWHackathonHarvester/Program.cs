using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MWHackathonHarvester.Services;
using MWHackathonHarvester.Feeds;

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
      
      var rijks = new RijksmuseumAmsterdam(db.GetFeed("Rijksmuseum Amsterdam"));
      var rijksEntries = rijks.GetEntries();
      db.SaveEntries(rijksEntries);


      Console.WriteLine("Done!");
      Console.ReadKey(true);
    }
  }
}
