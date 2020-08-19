using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedAt { get; set; }
    }
}
