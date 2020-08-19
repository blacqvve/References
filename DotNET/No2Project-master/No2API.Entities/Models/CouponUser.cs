using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
    public class CouponUser
    {
        [Key]
        public Guid Id { get; set; }
        public virtual CouponCode CouponCode { get; set; }
        public virtual User User { get; set; }
    }
}
