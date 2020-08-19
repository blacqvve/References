
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using WebMenu.Data.Data;
using WebMenu.Data.Misc.Enums;

namespace WebMenu.Data.Models
{
     public class Company
     {
          [Key]
          public Guid CompanyId { get; set; }

          public string CompanyName { get; set; }

          public string CompanyAdress { get; set; }

          public CompanyType CompanyType { get; set; }

          public virtual Menu Menu { get; set; }
          [MaxLength(127)]
          public virtual User User { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          public DateTime CreateDate { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          public DateTime ModifyDate  { get; set; }

          public bool Subscription { get; set; }
         public  Picture CompanyLogo { get; set; }

          public  Picture QRPicture { get; set; }

          public string LinkTag { get; set; }

          [NotMapped]
          public IFormFile File { get; set; }
     }
     public static class ModelExtensions
     {
          //public static IQueryable<Company> CompleteCompanies(this APIContext context)
          //{
          //     return context.Companies
          //     .Include(x => x.QRPicture)
          //     .Include(x => x.CompanyLogo)
          //     .Include(x => x.Menu)
          //     .ThenInclude(x => x.Categories)
          //     .ThenInclude(x => x.MenuItems)
          //     .ThenInclude(x => x.Pictures);
          //}
     }
}
