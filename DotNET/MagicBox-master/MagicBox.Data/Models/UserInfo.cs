using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Models
{
  public class UserInfo
  {
    [Key]
    public Guid Id { get; set; }
    public float Points { get; set; }
    public string City { get; set; }
    public int Gender { get; set; }
    [Column(TypeName = "TIMESTAMP")]
    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; }
    public virtual User User { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public byte[] Picture { get; set; }

    [NotMapped]
    public string Email { get; set; }
    [NotMapped]
    public string Password { get; set; }
    [NotMapped]
    [Display(Name = "Picture")]
    public IFormFile File { get; set; }
  }
}
