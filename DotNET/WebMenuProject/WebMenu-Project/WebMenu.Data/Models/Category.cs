using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebMenu.Data.Models
{
     public class Category
     {
          [Key]
          public Guid CategoryId { get; set; }

          public string CategoryName { get; set; }

          public string CategoryDescription { get; set; }

          public  Picture CategoryImage { get; set; }

          public bool Active { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          public DateTime CreateDate { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          public DateTime ModifyDate { get; set; }

          public virtual List<MenuItem> MenuItems { get; set; }

     }
}
