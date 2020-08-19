using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace No2API.Entities.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public string OrderId { get; set; }
    }
}
