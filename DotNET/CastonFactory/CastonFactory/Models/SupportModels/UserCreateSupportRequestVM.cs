using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models.SupportModels
{
     public class UserCreateSupportRequestVM
     {
          [Required(ErrorMessage ="Konu başlığı girmek zorunludur.")]
          [Display(Name ="Konu")]
          public string Subject { get; set; }

          [Required(ErrorMessage ="Mesaj bölümünü doldurmak zorunludur.")]
          [Display(Name ="Mesaj")]
          public string Message { get; set; }

          public string ContentID { get; set; }

     }
}
