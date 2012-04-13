using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
//using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;

namespace MWHackathonHarvester.Services
{
//  {
//   "photos":[
//      {
//         "url":"http:\/\/ahm.adlibsoft.com\/ahmimages\/A_1686.JPG",
//         "pid":"F@4b0831f6034dd93af883aa81e59c57bb_b9eaef84172552688d4f26d81e9238cf",
//         "width":269,
//         "height":400,
//         "tags":[
//            {
//               "tid":"TEMP_F@4b0831f6034dd93af883aa81e59c57bb_b9eaef84172552688d4f26d81e9238cf_49.26_22.38_0_0",
//               "recognizable":false,
//               "threshold":null,
//               "uids":[

//               ],
//               "gid":null,
//               "label":"",
//               "confirmed":false,
//               "manual":false,
//               "tagger_id":null,
//               "width":8.55,
//               "height":5.75,
//               "center":{
//                  "x":49.26,
//                  "y":22.38
//               },
//               "eye_left":{
//                  "x":48.46,
//                  "y":21.2
//               },
//               "eye_right":{
//                  "x":51.55,
//                  "y":20.94
//               },
//               "mouth_left":{
//                  "x":49.39,
//                  "y":23.39
//               },
//               "mouth_center":{
//                  "x":50.57,
//                  "y":23.38
//               },
//               "mouth_right":{
//                  "x":51.81,
//                  "y":23.25
//               },
//               "nose":{
//                  "x":50.78,
//                  "y":22.66
//               },
//               "ear_left":null,
//               "ear_right":null,
//               "chin":null,
//               "yaw":-14.8,
//               "roll":9.62,
//               "pitch":-2.51,
//               "attributes":{
//                  "face":{
//                     "value":"true",
//                     "confidence":72
//                  },
//                  "gender":{
//                     "value":"male",
//                     "confidence":59
//                  },
//                  "glasses":{
//                     "value":"false",
//                     "confidence":49
//                  },
//                  "smiling":{
//                     "value":"false",
//                     "confidence":94
//                  }
//               }
//            }
//         ]
//      }
//   ],
//   "status":"success",
//   "usage":{
//      "used":1,
//      "remaining":4999,
//      "limit":5000,
//      "reset_time_text":"Fri, 13 Apr 2012 00:11:31 +0000",
//      "reset_time":1334275891
//   },
//   "request_ids":[
//      "30e7ccfe572eeaadd5f6d1f8d06699ac"
//   ]
//}

  public class Coordinate
  {
    public double x { get; set; }
    public double y { get; set; }
  }

  public class Faces
  { 
     public Faces(string json, string url)
      : this(JObject.Parse(json), url)
    { 
    }

     public String Url { get; set; }
     //public int Proximity { get; set; }

    public Faces(JObject json, string url)
    {
      this.Url = url;
      this.Items = new List<Face>();

      var photo = json.SelectToken("photos")[0];
      var tags = photo.SelectToken("tags");
      this.Amount = tags.Count();
      foreach (JObject t in tags)
        this.Items.Add(new Face(t));

      Width = Convert.ToInt32(JSONService.GetText(photo, "width"));
      Height = Convert.ToInt32(JSONService.GetText(photo, "height"));
    }

    public int Width { get; set; }
    public int Height { get; set; }

    public int Amount { get; set; }
    public List<Face> Items {get;set;}
  }

  public class Face
  {
    public Face(JObject tag)
    {
      this.Width = Convert.ToDouble(JSONService.GetText(tag, "width").Replace(".",","));
      this.Height = Convert.ToDouble(JSONService.GetText(tag, "height").Replace(".", ","));
      var center = tag.SelectToken("center");
      this.Center = new Coordinate {
        x = Convert.ToDouble(JSONService.GetText(center, "x").Replace(".", ",")),
        y = Convert.ToDouble(JSONService.GetText(center, "y").Replace(".", ","))
      };
      var attr = tag.SelectToken("attributes");
      var gender = attr.SelectToken("gender");
      if (gender == null)
      {
        this.GenderConfidence = 0;
        this.Gender = false;
      }
      else
      {
        this.Gender = JSONService.GetText(gender, "value") == "male";
        this.GenderConfidence = Convert.ToInt32(JSONService.GetText(gender, "confidence"));
      }
    }

    //public int LookingDirection { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public Coordinate Center { get; set; }

    public bool Gender { get; set; }
    public int GenderConfidence { get; set; }


    //public Coordinate EyeLeft { get; set; }
    //public Coordinate EyeRight { get; set; }
    //public Coordinate MouthLeft { get; set; }
    //public Coordinate MouthCenter { get; set; }
    //public Coordinate MouthRight { get; set; }



  }

  public class FaceService
  { 
    

  }
}
