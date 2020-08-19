using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebMenu.Data.Data;
using WebMenu.Data.Managers;
using WebMenu.Data.Misc;
using WebMenu.Data.Misc.Enums;
using WebMenu.Data.Models;
using WebMenu.Manager.Models;

namespace WebMenu.Manager.Controllers
{
     public class HomeController : Controller
     {
          private readonly ILogger<HomeController> _logger;
          private readonly ICompanyManager _companyManager;
          private readonly IQrGenerateManager _qrGenerateManager;
          private readonly IWebHostEnvironment _env;
          private readonly APIContext _context;
          private readonly IHttpContextAccessor _httpContext;

          public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, ICompanyManager companyManager, IQrGenerateManager qrGenerateManager, APIContext context,IHttpContextAccessor httpContext)
          {
               _logger = logger;
               _env = env;
               _companyManager = companyManager;
               _qrGenerateManager = qrGenerateManager;
               _context = context;
               _httpContext = httpContext;

          }

          public IActionResult Index()
          {
               var company =  _companyManager.GetCompany(Guid.Parse("22e86408-75b8-4f3e-8d02-3fd0c00bc813"));
               return View(company.Result);
          }
          [HttpPost]
          public async Task<IActionResult> Index(int id)
          {
               var company = await _companyManager.GetCompany(Guid.Parse("22e86408-75b8-4f3e-8d02-3fd0c00bc813"));
               if (company == null)
                    return NotFound();
               string path = "http://www.vanillaunicornsoftware.com/" + company.LinkTag + "/" + (int)company.CompanyType;
               var result = await _qrGenerateManager.GenerateQrAsync(path, _env,Request);

               if (result.Return != ErrorReturns.Ok)
                    return BadRequest(new { Message = Helper.GetErrorMessage(result.Return) });

               _context.Pictures.Add(result.Object);
               await _context.SaveChangesAsync();
               company.QRPicture = result.Object;
               var update = await _companyManager.UpdateCompanyAsync(company);

               if (update.Return != ErrorReturns.Ok)
                    return BadRequest(new { Message = Helper.GetErrorMessage(result.Return) });


               return View(update.Object);
          }

          public async Task<IActionResult> Privacy(Company _company)
          {
               var company = new Company();
               if (ModelState.IsValid)
               {
                  
                    company.CompanyAdress = _company.CompanyAdress;
                    company.CompanyLogo = new Picture()
                    {
                         CreateDate = DateTime.Now,
                         PictureId = Guid.NewGuid(),
                         URL = "https://" + Request.Host.Value + _company.File.FileName
                    };
                    company.CompanyName = _company.CompanyName;
                    company.LinkTag = _company.LinkTag;
                    company.CompanyType = _company.CompanyType;
                    company.Menu = new Menu()
                    {
                         MenuId = Guid.NewGuid()
                    }; 
               }
               var action = await _companyManager.CreateCompanyAsync(company);

               var ac = await _companyManager.DeleteCompany(company.CompanyId);
               if (action.Return != ErrorReturns.Ok)
                    return BadRequest();

               return View(action.Object);
          }

          [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
          public IActionResult Error()
          {
               return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
          }
     }
}
