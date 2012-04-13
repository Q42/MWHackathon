using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWHackathonHarvester.Services;
using MWHackathon.Core.Models;
using System.Data.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace MWHackathonVisualizer.Controllers
{
  public class FacialController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Overlay(string url)
    {
      if (string.IsNullOrEmpty(url))
        url = "http://twimg0-a.akamaihd.net/profile_images/769005756/paulstork_foto_normal.png";

      string filename = @"c:\MWHackathon\Assets\uploaded\" + url.Substring(url.LastIndexOf('/') + 1);
      if (!System.IO.File.Exists(filename))
      {
        using (var client = new WebClient())
        {
          client.DownloadFile(url, filename);
        }
      }

      using (var db = new DatabaseService())
      {
        string json = db.FacialData(url);
        var upFaces = new Faces(json, url);

        int amount = upFaces.Amount;
        if (amount < 1)
          throw new Exception("no faces found");

        var dbEntries = db.GetAllEntries().Where(e => e.facial_amount == upFaces.Amount && (e.imageheight >= 350 || e.imagewidth >=350));
        dbEntries = dbEntries.Where(e => e.feed_id != 1); // not rijksmuseum, size klopt niet!
        var dbFaces = GetFaces(dbEntries.Take(100).ToList());
        //if (upFaces.Items.First().GenderConfidence > 50)
          //dbFaces = dbFaces.Where(f => f.Items.First().GenderConfidence > 50 && f.Items.First().Gender == upFaces.Items.First().Gender).ToList();


        var rnd = new Random();
        var dbFace = dbFaces.OrderBy(e => rnd.Next()).First();

        ViewBag.Faces = upFaces;
        return View(dbFace);

        //return Json(dbFaces, JsonRequestBehavior.AllowGet);
      }

    }

    private List<Faces> GetFaces(List<Entry> entries)
    {
      var result = new List<Faces>();

      foreach (var entry in entries)
      {
        result.Add(new Faces(entry.facialdata, entry.object_imageurl));
      }

      return result;
    }

  }
}
