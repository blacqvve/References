using CastonFactory.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CastonFactory.Data.Models
{
     public class SupportRequest
     {
          [Key]
          public Guid Id { get; set; }

          [Display(Name ="Oluşturan Kullanıcı")]
          public virtual User User { get; set; }

          [Display(Name ="Konu")]
          public string Subject { get; set; }

          [Display(Name ="Mesaj")]
          public string Message { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          [Display(Name ="Oluşturulma Tarihi")]
          public DateTime CreateDate { get; set; }

          [Column(TypeName = "TIMESTAMP")]
          [Display(Name ="Kapanma Tarihi")]
          public DateTime ClosingDate { get; set; }

          [Display(Name ="Durum")]
          public SupportState State { get; set; }

          [Display(Name ="İlgili İçerik")]
          public virtual Content Content { get; set; }



     }
}
