using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CastonFactory.Data.Models
{
     public class Genre
     {
          [Key]
          public Guid Id { get; set; }

          [Display(Name = "Tür İsmi")]
          public string Name { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          public DateTime CreateDate { get; set; }


          public bool UserGenre { get; set; }

          public virtual List<Content> Contents { get; set; }
     }
}
