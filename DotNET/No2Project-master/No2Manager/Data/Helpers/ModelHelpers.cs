using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace No2Manager.Data.Helpers
{
     public interface IModelHelpers
     {
          int MontlySubsCount();
          int UserCount();
     }
     public class ModelHelpers : IModelHelpers
     {
          private readonly ApplicationDbContext _context;
          public ModelHelpers(ApplicationDbContext context)
          {
               _context = context;
          }

          public int MontlySubsCount()
          {
               return _context.Users.Where(x => x.ActivationDate.Month == DateTime.Now.Month).Count();
          }
          public int UserCount()
          {
               return _context.Users.Where(x => x.HasPaid == true).Count();
          }
     }
}
