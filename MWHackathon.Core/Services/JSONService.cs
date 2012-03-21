using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace MWHackathonHarvester.Services
{
  public abstract class JSONService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(JSONService));

    public Feed Feed { get; set; }
    public abstract IEnumerable<JObject> GetObjects();
    public abstract string GetEntryId(JObject obj);
    public abstract string GetEntryName(JObject obj);
    public abstract string GetEntryImageUrl(JObject obj);

    /// <summary>
    /// parses OAI XML and returns each item individually
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerable<Entry> GetEntries()
    {
      DateTime downloadedDate = DateTime.Now;
      foreach (var obj in GetObjects())
      {
        string id = GetEntryId(obj);

        if (string.IsNullOrEmpty(id))
        {
          log.Error("No ID found for element, skipping...");
          continue;
        }

        string name = GetEntryName(obj);
        string imageurl = GetEntryImageUrl(obj);

        yield return new Entry
        {
          body = obj.ToString(),
          feed_id = Feed.id,
          lastimported_date = downloadedDate,
          object_id = id,
          object_name = name,
          object_imageurl = imageurl
        };

      }
    }

    public string GetText(JObject obj, string path)
    {
      string id = null;
      var idEl = obj.SelectToken(path, false);
      if (idEl != null)
        id = idEl.Value<string>();
      return id;
    }

  }
}
