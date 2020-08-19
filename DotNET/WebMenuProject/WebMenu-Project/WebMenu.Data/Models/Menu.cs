using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebMenu.Data.Models
{
     public class Menu
     {
          [Key]
          public Guid MenuId { get; set; }

          public string MenuName { get; set; }

          public virtual List<Category> Categories { get; set; }

          public virtual Company Company { get; set; }
          public Guid CompanyId { get; set; }

     }
}
