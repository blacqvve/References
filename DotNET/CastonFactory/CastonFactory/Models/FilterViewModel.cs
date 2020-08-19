using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models
{
     public class FilterViewModel
     {
          public string[] ContentTypes { get; set; }
          public List<Theme> Themes { get; set; }
          public bool SearchWithName { get; set; }

          public List<Content> Contents { get; set; }

     }
}
