using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
    public class Feed
    {
        [Key]
        public Guid Id { get; set; }
        public string Text { get; set; }
        public virtual User User { get; set; }
        public virtual Picture Picture { get; set; }
        public virtual Video Video { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedAt { get; set; }
    }
}
