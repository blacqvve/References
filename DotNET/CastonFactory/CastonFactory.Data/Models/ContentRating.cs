using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CastonFactory.Data.Models
{
     public class ContentRating
     {
          [Key]
          public Guid Id { get; set; }

          [Display(Name ="Görüntülenme Sayısı")]
          public int ViewCount { get; set; }

          [Display(Name = "Otoriter Puanı")]
          [Range(1, 10)]
          public int ManagerRating { get; set; } = 1;

          public Guid ContentID { get; set; }

     }
}
