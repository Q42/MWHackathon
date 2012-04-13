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
        //db.SaveEntries(new DeviantArt(db.GetFeed("DeviantArt")).GetSubmittableEntries(db));

        // gebleven bij het National Maritime Museum op http://museum-api.pbworks.com/w/page/21933420/Museum%C2%A0APIs


        //db.UpdateEntries(db.GetAllEntries(db.GetFeed("Amsterdam Museum").id), new AmsterdamMuseum(db.GetFeed("Amsterdam Museum")), img);
        //db.UpdateEntries(db.GetAllEntries(db.GetFeed("Cooper Hewitt").id), new CooperHewitt(db.GetFeed("Cooper Hewitt")), img);

        // in a museum
        //db.UploadVoorKars(db.GetAllEntries().Where(e => (e.object_imageurl != null) && e.imageheight != null && e.imageheight.Value > 0).OrderBy(e => e.feed_id).Select(e => e.id).ToList(), img, true);
        // not in a museum
        //db.UploadVoorKars(db.GetAllEntries(db.GetFeed("DeviantArt").id).Where(e => (e.object_imageurl != null) && e.imageheight != null && e.imageheight.Value > 0).OrderBy(e => e.feed_id).Select(e => e.id).ToList(), img, false);

        //db.ImageSizes(db.GetAllEntries().Where(e => (e.object_imageurl != null) && (e.imageheight == null || e.imagewidth == null)).OrderBy(e => e.feed_id).Select(e => e.id).ToList(), img);

        // download images

        //foreach (var entry in db.GetAllEntries(db.GetFeed("DeviantArt").id).Where(e => e.object_imageurl != null && e.object_imageurl != "404").OrderByDescending(e => e.feed_id))
          //db.DownloadImage(entry, img);

        // facial recognition
        db.RecognizeFaces(db.GetAllEntriesWithImages().Where(e => e.facialdata == null).Select(e => e.id).ToList());
        //db.FacesAmount(db.GetAllEntriesWithImages().Where(e => e.facialdata != null && e.facial_amount == null).Select(e => e.id).ToList());

        //int max = db.GetAllEntries().Count();
        //int count = 0;
        //int per = percentage(count, max);
        //foreach (var e in db.GetAllEntries().Skip(300000))
        //{
        //  var file = img.GetFilePath(e);
        //  var filepath = file + ".txt";

        //  count++;

        //  int newper = percentage(count, max);
        //  if (per != newper)
        //  {
        //    log.Info(newper + "%");
        //    per = newper;
        //  }

        //  if (!File.Exists(filepath))
        //  {
        //    log.Info("writing to " + filepath);
        //    File.WriteAllText(filepath, e.body);
        //  }
        //}
      }

      Console.WriteLine("Done!");
      Console.ReadKey(true);
    }

    private static int percentage(int done, int max)
    {
      return Convert.ToInt32(((double)done / (double)max) * 100);
    }

  }
}
