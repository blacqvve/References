using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CastonFactory.Data.Models
{
    public class User :IdentityUser
     {
         
          [Display(Name = "Ad")]
          public string Name { get; set; }
          [Display(Name = "Soyad")]
          public string Surname { get; set; }
          [Display(Name = "İletişim")]
          public override string PhoneNumber { get; set; }
          public virtual List<Content> Contents { get; set; }

          public virtual List<SupportRequest> SupportRequests { get; set; }

          [NotMapped]
          [Display(Name="Üretici İsmi")]
          public string FullName { get {
                    return Name + " " + Surname;
               }
          }
     }
}
