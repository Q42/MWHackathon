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

      //db.SaveEntries(new RijksmuseumAmsterdam(db.GetFeed("Rijksmuseum Amsterdam")).GetEntries());
      //db.SaveEntries(new Powerhouse(db.GetFeed("Powerhouse")).GetEntries());
      //db.SaveEntries(new BrooklynMuseum(db.GetFeed("BrooklynMuseum")).GetEntries());


      Console.WriteLine("Done!");
      Console.ReadKey(true);
    }
  }
}
