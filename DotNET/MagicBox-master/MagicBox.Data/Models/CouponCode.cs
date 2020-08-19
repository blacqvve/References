using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Models
{
  public class CouponCode
  {
    [Key]
    public Guid Id { get; set; }
    public virtual User User { get; set; }
    public virtual User Creator { get; set; }
    public virtual Picture Picture { get; set; }
    public string Code { get; set; }
    public bool Active { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    [Column(TypeName = "TIMESTAMP")]
    public DateTime ExpiresAt { get; set; }
    [Column(TypeName = "TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
  }
}
