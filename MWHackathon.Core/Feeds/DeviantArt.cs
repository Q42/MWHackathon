using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using MWHackathonHarvester.Services;
using System.Configuration;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MWHackathonHarvester.Feeds
{
  public class DeviantArt : XMLService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(DeviantArt));

    public DeviantArt(Feed feed)
    {
      this.Feed = feed;
    }

    private static string url = "http://backend.deviantart.com/rss.xml?type=deviation&q=boost%3Apopular+in%3Aphotography+";
    private static List<string> searchterms = new List<string>() { 
      "frog",
      "monkey",
      //"motorcycle", 
      //"harley",
      "airplane",
      //"art",
      "color",
      "shape",
      "abstract",
      "form",
      "drawing",
      "coffee",
      "beautiful",
      "urban",
      "pose",
      "ugly",
      "today",
      "colour",
      "power", 
      "page", 
      "paint",
      "tea", 
      "happy",
      "truck",
      "flowers", "grey", "pink", "red", "blue", "violet", "green", "fuchsia", "orange", "black"

    };

    List<string> guids = new List<string>() { 
      "http://3dueces.deviantart.com/art/Chevy-Truck-v2-2045817",
      "http://52d.deviantart.com/art/Beautiful-128279397",
      "http://adapho.deviantart.com/art/shape-133520185", 
      "http://livmalene.deviantart.com/art/frog-43785091", 
      "http://lieveheersbeestje.deviantart.com/art/Mr-frog-291578743", 
      "http://lahavana.deviantart.com/art/Shape-68204929",
      "http://gilad.deviantart.com/art/The-Shape-Of-City-Winter-91674310",
      "http://monroemisfit.deviantart.com/art/Abstract-145946412",
      "http://NachoRomero.deviantart.com/art/Abstract-269350357",
      "http://Nakedthoughts.deviantart.com/art/Abstract-88743309",
      "http://Jules1983.deviantart.com/art/Abstract-181372067",
      "http://clyme.deviantart.com/art/Orange-72134804",
      "http://DianePhotos.deviantart.com/art/Orange-158482652",
      "http://dejanrushdate.deviantart.com/art/ORANGE-RULES-56107310",
      "http://markmarkmark.deviantart.com/art/Orange-86089187",
      "http://Lucem.deviantart.com/art/Orange-juice-47478875",
      "http://laura242.deviantart.com/art/Orange-116447677",
      "http://dingodave.deviantart.com/art/Urban-Ghost-Trees-Redux-187675703",
      "http://justeline.deviantart.com/art/Under-the-Red-109534869",
      "http://Jellings.deviantart.com/art/Red-18340001",
      "http://gilad.deviantart.com/art/Feeling-Red-52402448",
      "http://lieveheersbeestje.deviantart.com/art/Red-roses-263008314",
      "http://Jez92.deviantart.com/art/Red-52582316",
      "http://pacsaman.deviantart.com/art/Red-Queen-166166159",
      "http://AtXU.deviantart.com/art/Black-White-46840585",
      "http://mariemadame.deviantart.com/art/Black-and-White-57547870",
      "http://Jenvanw.deviantart.com/art/Zebra-Black-and-White-84987030",
      "http://Vikarus.deviantart.com/art/Black-and-white-191004743",
      "http://ugurers.deviantart.com/art/black-white-35288440",
      "http://TheCauseOf.deviantart.com/art/in-black-and-white-49930576",
      "http://mechtaniya.deviantart.com/art/Rabbit-with-a-violet-flower-212970530",
      "http://Anonymous-Art.deviantart.com/art/Zebra-Abstract-105314180",
      "http://atomb0mb.deviantart.com/art/Abstract-86404076",
      "http://Blazedezignz.deviantart.com/art/Abstract-70156249",
      "http://DeKrooked.deviantart.com/art/Abstract-57033222",
      "http://Davincigirl.deviantart.com/art/Abstract-109246732",
      "http://contorted4life.deviantart.com/art/Abstract-196604958",
      "http://GVA.deviantart.com/art/Urban-Abstract-IV-43038572",
      "http://TchaikovskyCF.deviantart.com/art/A-More-Abstract-Flower-86640765",
      "http://pola12345678910.deviantart.com/art/abstract-105121972",
      "http://mstargazer.deviantart.com/art/Abstract-Lily-119185222",
      "http://parosimonas.deviantart.com/art/abstract-64468531",
      "http://anneeatsworms.deviantart.com/art/airplane-88737597",
      "http://drakke.deviantart.com/art/Airplane-42330590",
      "http://Jh2.deviantart.com/art/airplane-85663971",
      "http://Grofica.deviantart.com/art/Airplane-98243291",
      "http://LockedIllusions.deviantart.com/art/Airplane-186233405",
      "http://celtic-ronin.deviantart.com/art/Drawing-out-the-Ghost-7444360",
      "http://Jack070.deviantart.com/art/Drawing-124482074",
      "http://rain1man.deviantart.com/art/Drawing-123565856",
      "http://picatoo.deviantart.com/art/drawing-200681982",
      "http://flyinglikesuperman.deviantart.com/art/You-are-beautiful-166402806",
      "http://steLLinaVELENOSA.deviantart.com/art/beautiful-68979191",
      "http://chiyo-tsukiko.deviantart.com/art/flowers-84761742",
      "http://Joey-2000.deviantart.com/art/Flowers-109381428",
      "http://photographymienn1.deviantart.com/art/Flowers-60729776",
      "http://TigerGod.deviantart.com/art/Flowers-15553730",
      "http://PrinceHyde.deviantart.com/art/Flowers-62230442",
      "http://andreydubinin.deviantart.com/art/Coffee-60564155",
      "http://armony20.deviantart.com/art/Coffee-42443078",
      "http://c-inderella.deviantart.com/art/Coffee-49408158",
      "http://Lestrovoy.deviantart.com/art/Coffee-37477698",
      "http://MADemoseille.deviantart.com/art/coffee-90727499",
      "http://fugit.deviantart.com/art/coffee-36815930",
      "http://Katari01.deviantart.com/art/coffee-194877491",
      "http://justeline.deviantart.com/art/Wake-up-and-Smell-the-Coffee-98010376",
      "http://Kukuruki.deviantart.com/art/Coffee-and-cigarettes-112746081",
      "http://Teayii.deviantart.com/art/Coffee-277486258"
    };

    public override IEnumerable<XmlDocument> GetPagedXml()
    {
      foreach (var item in searchterms)
      {
        yield return Utils.DownloadXml(url + item);
      }
    }

    public override string XPathToRecord
    {
      get
      {
        int count = 0;
        string result = "/rss/channel/item";
        //return result;
        result += "[";
        foreach (var guid in guids)
        {
          if (count != 0)
            result += " or ";
          result += "guid='" + guid + "'";
          count++;
        }
        result += "]";
        return result;
      }
    }

    public override string GetEntryId(XmlElement el)
    {
      return GetXpathInnerText(el, "guid");
    }

    public override string GetEntryName(XmlElement el)
    {
      return GetXpathInnerText(el, "title");
    }

    public override int GetImageHeight(XmlElement el)
    {
      var img = GetEntryImageElement(el);
      if (img == null)
        return 0;
      return Convert.ToInt32(img.GetAttribute("height"));
    }
    public override int GetImageWidth(XmlElement el)
    {
      var img = GetEntryImageElement(el);
      if (img == null)
        return 0;
      return Convert.ToInt32(img.GetAttribute("width"));
    }

    public XmlElement GetEntryImageElement(XmlElement el)
    {
      var els = el.SelectNodes("*[local-name()='thumbnail' or (local-name()='content' and @medium='image')]");

      int max = 0;
      foreach (XmlElement item in els)
      {
        int width = Convert.ToInt32(item.GetAttribute("width"));
        max = Math.Max(max, width);
        int height = Convert.ToInt32(item.GetAttribute("height"));
        max = Math.Max(max, height);
      }

      foreach (XmlElement item in els)
      {
        int width = Convert.ToInt32(item.GetAttribute("width"));
        int height = Convert.ToInt32(item.GetAttribute("height"));
        if (width == max || height == max)
          return item;
      }

      return null;
    }

    public override string GetEntryImageUrl(XmlElement el)
    {
      var img = GetEntryImageElement(el);
      if (img == null)
        return null;
      return img.GetAttribute("url");
    }
  }
}
