using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
    public class Sale
    {
        [Key]
        public Guid Id { get; set; }
        public virtual User Salesman { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
