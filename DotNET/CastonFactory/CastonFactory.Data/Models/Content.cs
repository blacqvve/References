using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CastonFactory.Data.Models
{
     public class Content
     {
          [Key]
          public Guid Id { get; set; }

          [Display(Name ="Eser Adı")]
          public string CreationName { get; set; }

          public virtual User User { get; set; }


          [Display(Name ="İçerik Tipi")]
          public string ContentType { get; set; }

          [Display(Name ="Tema")]
          public Theme Theme { get; set; }

          [Display(Name ="Tür")]
          public Genre Genre { get; set; }

          public virtual ContentData Data { get; set; }
          
          public virtual ContentRating Rating { get; set; }

          [Display(Name ="Aktiflik Durumu")]
          public bool IsActive { get; set; }

          [Display(Name ="Premium Durumu")]
          public bool IsPremium { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          [Display(Name ="Eklenme Tarihi")]
          public DateTime CreateDate { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          [Display(Name = "Düzenlenme Tarihi")]
          public DateTime ModifyDate { get; set; }

          [Display(Name ="Ödeme Durumu")]
          public bool Paid { get; set; }

          public bool isEdited { get; set; }

          [NotMapped]
          public IFormFile  file { get; set; }
     }
}
