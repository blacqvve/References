using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QRCoder;
using WebMenu.Data.Data;
using WebMenu.Data.Managers;
using WebMenu.Data.Misc;
using WebMenu.Data.Misc.Enums;
using WebMenu.Data.Models;



namespace WebMenu.Web.Controllers
{

    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ICompanyManager companyManager;
        private readonly APIContext context;
        private readonly IHostingEnvironment env;

        public HomeController(ILogger<HomeController> logger, ICompanyManager _companyManager, APIContext _context, IHostingEnvironment env)
        {
            _logger = logger;
            companyManager = _companyManager;
            context = _context;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Index(string QrCodeLink)
        {
            

            return View();
        }


        [Route("Menu/{tag}/{int}")]
        public async Task<IActionResult> Coffee(string tag, int companyType)
        {
            var com = await context.Companies.FirstOrDefaultAsync(x => x.LinkTag == tag);
            var company = await companyManager.GetCompany(com.CompanyId);
            company.CompanyType = (CompanyType)companyType;
            if (company == null)
                return NotFound();
            var result = await companyManager.GetCompanyMenuAsync(company);
            if (result.Return != ErrorReturns.Ok)
                return NotFound();
            company.Menu = result.Object;

            return View("Views/Menus/Coffee.cshtml", company);
        }

        public IActionResult Foody(Company _company)
        {
            var company = new Company();
            company = _company;
            var result = companyManager.CreateCompanyAsync(company);

            if (result.Result.Return != ErrorReturns.Ok)
            {
                //TODO:Error handle
                return NotFound(new { Message = Helper.GetErrorMessage(result.Result.Return) });
            }
            return View("Views/Menus/Foody.cshtml");
        }



        public IActionResult Luto()
        {

            return View("Views/Menus/Luto.cshtml");
        }


        public IActionResult RedcayenneList()
        {

            return View("Views/Menus/RedcayenneList.cshtml");
        }

        public IActionResult RedcayenneGrid()
        {

            return View("Views/Menus/RedcayenneGrid.cshtml");
        }


        public IActionResult Tasty()
        {

            return View("Views/Menus/Tasty.cshtml");
        }
    }
}
