using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWHackathonHarvester.Services;
using MWHackathon.Core.Models;
using System.Data.Linq;
using System.Configuration;
using System.IO;

namespace MWHackathonVisualizer.Controllers
{
  public class ImageController : Controller
  {

    private static DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["assertlocation"]);

    public ActionResult Index(int feedid, int entryid)
    {
      using (var db = new DatabaseService())
      {
        var img = new ImageService(dir, db.GetAllFeeds().ToList());
        var file = img.GetFilePath(feedid, entryid, ".jpg");
        if (file == null || !file.Exists)
          file = img.GetFilePath(feedid, entryid, ".bmp");
        if (file == null || !file.Exists)
          file = img.GetFilePath(feedid, entryid, ".gif");
        if (file == null || !file.Exists)
          file = img.GetFilePath(feedid, entryid, ".png");
        if (file == null || !file.Exists)
          throw new FileNotFoundException();
        Response.WriteFile(file.FullName);

        Response.ContentType = "image/jpeg";

        return new EmptyResult();
      }
    }

  }
}
