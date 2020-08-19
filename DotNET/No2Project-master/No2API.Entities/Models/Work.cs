using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
  public class Work
  {
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public virtual User Creator { get; set; }
    [Column(TypeName = "TIMESTAMP")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "TIMESTAMP")]
    public DateTime? ClosingDate { get; set; }
    public bool ActiveState { get; set; }
    public virtual List<WorkApplications> Applications { get; set; }

  }
}
