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
using System.Drawing;
using System.Net;

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


    public ActionResult External(string url)
    {
      // first download
      string filename = @"c:\MWHackathon\Assets\uploaded\" + url.Substring(url.LastIndexOf('/') + 1);
      if (!System.IO.File.Exists(filename))
      {
        using (var client = new WebClient())
        {
          client.DownloadFile(url, filename);
        }
      }

      // then load
      //Image g = Bitmap.FromFile(filename);

      // then output
      Response.WriteFile(filename);
      if (filename.Contains("png"))
        Response.ContentType = "image/png";
      else
        Response.ContentType = "image/jpeg";
      return new EmptyResult();
    }
  }
}
