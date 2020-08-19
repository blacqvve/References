using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
  public class WorkApplications
  {
    [Key]
    public Guid Id { get; set; }

    public virtual User Applicant { get; set; }

    public virtual Work Work { get; set; }

    [Column(TypeName = "TIMESTAMP")]
    public DateTime ApplyDate { get; set; }
  }
}
