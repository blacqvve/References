using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CastonFactory.Data.Models
{
     public class ContentData
     {
          [Key]

          public Guid Id { get; set; }

          public string Link { get; set; }

          public string FileExtension
          {
               get
               {
                    if (!string.IsNullOrEmpty(Link))
                    {
                         return System.IO.Path.GetExtension(Link);
                    }
                    return FileExtension;

               }
               set { }
          }
          public Guid ContentId { get; set; }

     }
}
