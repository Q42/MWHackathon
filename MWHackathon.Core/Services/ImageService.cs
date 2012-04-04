using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;

namespace MWHackathonHarvester.Services
{
  public class ImageService
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(ImageService));
    private readonly DirectoryInfo dir;
    private readonly List<Feed> feeds;

    public ImageService(DirectoryInfo dir, List<Feed> feeds)
    {
      this.dir = dir;
      this.feeds = feeds;
    }

    public void DownloadImage(string url, FileInfo file)
    {
      if (string.IsNullOrEmpty(url) || file == null)
        return;

      if (!file.Directory.Exists)
        file.Directory.Create();

      using (WebClient web = new WebClient())
      {
        try
        {
          url = url.Replace("&aria/maxwidth_288", "&1024w");
          web.DownloadFile(url, file.FullName);
          //log.DebugFormat("Downloading {0} to {1}", url, file.FullName);
        }
        catch (WebException ex)
        {
          if (ex.Message.Contains("(404)"))
          {
            throw new FileNotFoundException("Url not found: " + url);
          }
          else
            log.Error("Unable to download " + url, ex);
        }
      }
    }

    public FileInfo GetFilePath(Entry entry)
    {
      if (string.IsNullOrEmpty(entry.object_imageurl))
        return null;
      //Feed feed = feeds.Single(f => f.id == entry.feed_id);
      string entryId = entry.id.ToString().PadLeft(6,'0');
      string extension = Path.GetExtension(entry.object_imageurl);

      // rijksmuseum       "http://www.rijksmuseum.nl/assetimage2.jsp?id=SK-A-4878&aria/maxwidth_288"
      if (entry.object_imageurl.Contains("www.rijksmuseum.nl") && string.IsNullOrEmpty(extension))
        extension = ".jpg";

      if (extension.Contains("?"))
        extension = extension.Split('?')[0];

      if (extension.ToLowerInvariant() != ".jpg" && extension.ToLowerInvariant() != ".png" && extension.ToLowerInvariant() != ".bmp" && extension.ToLowerInvariant() != ".gif")
        log.Warn("Invalid extension found: " + extension);
      if (extension.Length != 4)
        log.Warn("Extension wrong length: " + extension);

      return new FileInfo(Path.Combine(dir.FullName, entry.feed_id.ToString(), entryId.Substring(0, 2), entryId.Substring(0, 3), entryId + extension));
    }


    /// <summary>
    /// return dictionary with fullrow and split by separator
    /// </summary>
    /// <param name="file"></param>
    /// <param name="separator"></param>
    /// <param name="firstLineContainsHeaders"></param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, List<string>>> ParseCSV(FileInfo file, string separator, bool firstLineContainsHeaders)
    {
      string fileName = file.FullName;
      string cacheKey = fileName + separator;
      int count = 0;

      //make sure you export the CSV file with excel, open it with notepad afterwards and save it as UTF8 encoding

      using (TextReader tr = new StreamReader(fileName, System.Text.Encoding.UTF8))
      {
        for (var line = ReadRow(tr, separator); line.Key != null; line = ReadRow(tr, separator))
        {
          count++;
          if (firstLineContainsHeaders && count == 1)
            continue;

          yield return line;
        }
      }
    }

    private static KeyValuePair<string, List<string>> ReadRow(TextReader tr, string separator)
    {
      string pattern = String.Format("(^|{0})(\"(([^\"]|\"\")*)\"|[^{0}]*)", separator);
      List<string> results = new List<string>();
      string line = tr.ReadLine();

      if (line == null)
        return new KeyValuePair<string, List<string>>(line, results);

      for (Match m = Regex.Match(line, pattern); m.Success; m = m.NextMatch())
        results.Add(m.Groups[3].Success
          ? m.Groups[3].Value.Replace("\"\"", "\"")
          : m.Groups[2].Value);

      return new KeyValuePair<string, List<string>>(line, results);
    }

  }
}
