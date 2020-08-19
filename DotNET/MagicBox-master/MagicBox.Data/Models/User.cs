using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Models
{
  public class User:IdentityUser
  {
    //Not Mapped
    //public List<CouponCode> CouponCodes { get; set; }
    //public List<CouponCode> CreatedCoupons { get; set; }
    public List<UserItem> UserItems { get; set; }

    public virtual UserInfo UserInfo { get; set; }
  }
}
