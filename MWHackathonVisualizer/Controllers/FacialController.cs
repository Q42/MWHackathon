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
        url = "http://img2.timeinc.net/people/i/2011/specials/beauties/mag/jennifer-lopez-435.jpg";

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
        var faces = new Faces(json);
        return Json(faces, JsonRequestBehavior.AllowGet);
      }

    }

  }
}
