using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWHackathon.Core.Models;
using log4net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace MWHackathonHarvester.Services
{
  public static class Utils
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(Utils));

    public static XmlDocument DownloadXml(string url)
    {
      log.DebugFormat("Downloading {0}", url);

      XmlDocument doc = new XmlDocument();
      doc.Load(url);
      return doc;
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
