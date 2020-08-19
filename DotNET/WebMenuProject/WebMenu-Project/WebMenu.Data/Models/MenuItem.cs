using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebMenu.Data.Models
{
     public class MenuItem
     {
          [Key]
          public Guid ItemId { get; set; }

          public string  ItemName { get; set; }

          public string ItemDescription { get; set; }

          public decimal ItemPrice { get; set; }

          public bool Active { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          public DateTime CreateDate { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          public DateTime ModifyDate { get; set; }

          public virtual List<Picture> Pictures { get; set; }


     }
}
