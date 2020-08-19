using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Models
{
    public class UserContentEditVM
    {
    


        [Display(Name = "Eser Adı")]
        [Required(ErrorMessage = "Bu Alan Zorunludur")]
        public string CreationName { get; set; }

        [Display(Name = "İçerik Tipi")]
        [Required(ErrorMessage = "Bu Alan Zorunludur")]
        public string ContentType { get; set; }


        [Display(Name = "Tema")]
        public string Theme { get; set; }

        [Display(Name = "Tema")]
        public string NewTheme { get; set; }

        [Display(Name = "Tür")]
        public string Genre { get; set; }

        [Display(Name = "Tür")]
        public string NewGenre { get; set; }


    }
}
