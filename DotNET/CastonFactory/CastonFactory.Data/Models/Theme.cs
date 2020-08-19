using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CastonFactory.Data.Models
{
    public class Theme
     {
          [Key]
          public Guid Id { get; set; }
          
          [Display(Name="Tema İsmi")]
          public string Name { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          public DateTime CreateDate { get; set; }

          public virtual List<Content> Contents { get; set; }

          public bool UserTheme { get; set; }

     }
}
