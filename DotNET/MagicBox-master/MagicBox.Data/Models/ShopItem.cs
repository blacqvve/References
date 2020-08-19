using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Models
{
  public class ShopItem
  {
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual CouponCode CouponCode { get; set; }
    public List<UserItem> UserItems { get; set; }
  }
}
