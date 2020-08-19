using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models
{
    public class UserVM
    {

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Ad")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Soyad")]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en az  {2} ve en fazla {1} uzunlukta olmalı.", MinimumLength = 8)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon Numaranız")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-Posta Adresi")]
        public string Email { get; set; }


    }
}
