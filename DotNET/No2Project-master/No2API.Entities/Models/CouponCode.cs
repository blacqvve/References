using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
    public class CouponCode
    {
        [Key]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; } = new decimal (0.0);
        public bool Active { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime? ExpiresAt { get; set; } = null;
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedAt { get; set; }
    }
}
