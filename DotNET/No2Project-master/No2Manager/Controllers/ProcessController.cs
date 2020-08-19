using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using No2Manager.Data;

namespace No2Manager.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProcessController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public ProcessController(ApplicationDbContext context)
    {
      _context = context;
    }

    [HttpGet ("SearchUsers")]
    public IActionResult SearchUsers(string email)
    {
      return Ok(_context.Users.Where(x => x.Email.Contains(email)).Select(x=> new { x.Id, x.Email  }).ToList());
    }
  }
}