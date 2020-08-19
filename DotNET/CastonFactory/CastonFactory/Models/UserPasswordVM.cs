using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models
{
    public class UserPasswordVM
    {
        [DataType(DataType.Password)]
        [Display(Name = "Eski Şifreniz")]
        [Compare("Password", ErrorMessage = "Eski şifreniz yanlış.")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en az  {2} ve en fazla {1} uzunlukta olmalı.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
