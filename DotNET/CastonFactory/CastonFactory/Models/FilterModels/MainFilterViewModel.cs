using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models.FilterModels
{
     public class MainFilterViewModel
     {
          public string Genre { get; set; }

          public string Theme { get; set; }

          public string[] ContentTypes { get; set; }

          public string Keyword { get; set; }

     }
}
