using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebMenu.Data.Models
{
     public class Picture
     {
          [Key]
          public Guid PictureId { get; set; }

          public string URL { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          public DateTime CreateDate { get; set; }
     }
}
