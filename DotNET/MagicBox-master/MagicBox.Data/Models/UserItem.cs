using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Models
{
  public class UserItem
  {
    [Key]
    public int Id { get; set; }
    public string PlayerId { get; set; }
    public virtual User Player { get; set; }
    public Guid ShopItemId { get; set; }
    public virtual ShopItem ShopItem { get; set; }
  }
}
