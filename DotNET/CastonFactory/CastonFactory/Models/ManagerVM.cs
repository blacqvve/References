using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models
{
    public class ManagerVM
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Kullanıcı Adı")]
        [StringLength(14, ErrorMessage = "{0} en az {2} ve en fazla {1} uzunlukta olmalı", MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en az  {2} ve en fazla {1} uzunlukta olmalı.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-Posta Adresi")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Ad")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Soyad")]
        public string Surname { get; set; }
    }
}
