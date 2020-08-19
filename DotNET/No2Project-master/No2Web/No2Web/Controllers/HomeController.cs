using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using No2Web.Data;
using No2API.Entities.Models;

namespace No2Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext context;

        public HomeController(DataContext context)
        {
            this.context = context;
        }

        //[Route("Index")]
        public IActionResult Index()
        {
            return NotFound();
        }

        //[Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
