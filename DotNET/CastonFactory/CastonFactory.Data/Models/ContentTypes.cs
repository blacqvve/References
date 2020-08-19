using System;
using System.Collections.Generic;
using System.Text;

namespace CastonFactory.Data.Models
{
     public class ContentTypes
     {
          public const string Composition = "Beste";
          public const string Lyrics = "Söz";
          public const string Infrastructure = "Altyapı";
          public const string Cover = "Cover";
          public const string Remix_Pool = "Remix";

          public static  string[] GetContentTypes()
          {
               return new string[5] { Composition, Lyrics, Infrastructure, Cover, Remix_Pool };
          }
     }
}
