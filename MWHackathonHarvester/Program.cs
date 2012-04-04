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

      using (var db = new DatabaseService())
      {
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
        db.SaveEntries(new StedelijkMuseum(db.GetFeed("Stedelijk Museum Amsterdam")).GetSubmittableEntries(db));
        
        // gebleven bij het National Maritime Museum op http://museum-api.pbworks.com/w/page/21933420/Museum%C2%A0APIs




        // download images

        DirectoryInfo dir = new DirectoryInfo("App_Data/img");
        var img = new ImageService(dir, db.GetAllFeeds().ToList());
        foreach (var entry in db.GetAllEntries().Where(e => e.object_imageurl != null && e.object_imageurl != "404").OrderByDescending(e => e.feed_id))
        {
          var file = img.GetFilePath(entry);
          if (file != null && !file.Exists)
          {
            try
            {
              img.DownloadImage(entry.object_imageurl, file);
              Console.Write(".");
            }
            catch (FileNotFoundException)
            {
              log.Info("Image not found: " + entry.object_imageurl);
            }
          }
        }
      }

      Console.WriteLine("Done!");
      Console.ReadKey(true);
    }
  }
}
