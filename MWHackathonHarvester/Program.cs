using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
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

      DirectoryInfo dir = new DirectoryInfo("c:\\MWHackathon\\Assets");


      using (var db = new DatabaseService())
      {
        var img = new ImageService(dir, db.GetAllFeeds().ToList());

        //db.DeleteEverything();

        //db.SaveEntries(new CooperHewitt(db.GetFeed("Cooper Hewitt")).GetSubmittableEntries(db));
        //db.SaveEntries(new RijksmuseumAmsterdam(db.GetFeed("Rijksmuseum Amsterdam")).GetSubmittableEntries(db));
        //db.SaveEntries(new BrooklynMuseum(db.GetFeed("Brooklyn Museum")).GetSubmittableEntries(db));
        //db.SaveEntries(new Powerhouse(db.GetFeed("Powerhouse")).GetSubmittableEntries(db));
        //db.SaveEntries(new ScienceMuseum(db.GetFeed("Science Museum London")).GetSubmittableEntries(db));
        //db.SaveEntries(new AmsterdamMuseum(db.GetFeed("Amsterdam Museum")).GetSubmittableEntries(db));

        //db.SaveEntries(new MuseumVictoria(db.GetFeed("Museum Victoria")).GetSubmittableEntries(db));
        //db.SaveEntries(new VictoriaAndAlbertMuseum(db.GetFeed("Victoria & Albert Museum")).GetSubmittableEntries(db));

        //db.SaveEntries(new WestAustralia(db.GetFeed("Western Australian Museum")).GetSubmittableEntries(db));
        //db.SaveEntries(new StedelijkMuseum(db.GetFeed("Stedelijk Museum Amsterdam")).GetSubmittableEntries(db));
        
        // gebleven bij het National Maritime Museum op http://museum-api.pbworks.com/w/page/21933420/Museum%C2%A0APIs


        db.UpdateEntries(db.GetAllEntries(db.GetFeed("Amsterdam Museum").id), new AmsterdamMuseum(db.GetFeed("Amsterdam Museum")), img);
        db.UpdateEntries(db.GetAllEntries(db.GetFeed("Cooper Hewitt").id), new CooperHewitt(db.GetFeed("Cooper Hewitt")), img);


        // download images

        //foreach (var entry in db.GetAllEntries().Where(e => e.object_imageurl != null && e.object_imageurl != "404").OrderByDescending(e => e.feed_id))
        //  db.DownloadImage(entry);
      }

      Console.WriteLine("Done!");
      Console.ReadKey(true);
    }
  }
}
