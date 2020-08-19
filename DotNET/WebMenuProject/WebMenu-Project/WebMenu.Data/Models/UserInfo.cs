using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebMenu.Data.Models
{
     public class UserInfo
     {
          [Key]
          public Guid Id { get; set; }

          public string City { get; set; }

          [Column(TypeName ="TIMESTAMP")]
          public DateTime CreateDate { get; set; }
          [MaxLength(length:127)]
          public string UserId { get; set; }

          public virtual User User { get; set; }

          [NotMapped]
          public string Email { get; set; }
          [NotMapped]
          public string Password { get; set; }
     }
}
