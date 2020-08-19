using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using WebMenu.Data.Data;
using WebMenu.Data.Managers;
using WebMenu.Data.Misc;
using WebMenu.Data.Misc.Enums;
using WebMenu.Data.Models;

namespace WebMenu.API.Controllers
{
    [Route("api/v0/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
          private  readonly ILogger<CompaniesController> logger;
          private  readonly APIContext context;
          private readonly ICompanyManager companyManager;

          public CompaniesController(APIContext _context,ILogger<CompaniesController> _logger,ICompanyManager _companyManager)
          {
               context = _context;
               logger = _logger;
               companyManager = _companyManager;
          }

          [HttpGet("Check")]
          public IActionResult Get()
          {
               logger.LogInformation("logger operational");
               return Ok("api operational");
          }
          [HttpGet("{id}")]
          public async Task<IActionResult> GetCompany(Guid id)
          {
               var company = await companyManager.GetCompanyMenuAsync(id);
               if (company.Object == null)
                    return BadRequest(new { Message = Helper.GetErrorMessage(ErrorReturns.NotFound)});
               return Ok(company.Object);
          }

          [HttpPost("Create")]
          public async Task<IActionResult> CreateCompany([FromBody]Company company)
          {
               company.CompanyId = Guid.NewGuid();
               company.CreateDate = DateTime.Now;
               company.ModifyDate = DateTime.Now;
               var returns = await companyManager.CreateCompanyAsync(company);
               if (returns.Return==ErrorReturns.Ok)
               {
                    logger.LogInformation("Create company success", returns.Object.CompanyId);
                    return Ok(company.CompanyId);
               }
               return BadRequest(new { Message = Helper.GetErrorMessage(returns.Return)});
               
          }

          [HttpDelete("Delete")]
          public async Task<IActionResult> DeleteCompany([FromQuery]Guid id)
          {

               var action = await companyManager.DeleteCompany(id);
               if (action != ErrorReturns.Ok)
                    return BadRequest(new { Message = Helper.GetErrorMessage(action) });

               return Ok(id+"deleted");
          }

         [HttpGet("Seed")]
         public async Task<IActionResult> Seed(int count)
          {
               var result = await Helper.SeedDataToDatabase(context, logger, count);
               if (!result)
               {
                    return BadRequest();
               }
               return Ok();
          }

          [HttpGet("GetMenu/{id}")]
          public async Task<IActionResult> GetCompanyMenu(Guid id)
          {
               EventId eventId = new EventId(1, "Get Company Menu");
               
               logger.LogInformation(eventId, "Started",$"CompanyID: {id}");
               var result = await companyManager.GetCompanyMenuAsync(id);
               if (result.Return!=ErrorReturns.Ok)
               {
                    logger.LogError(eventId, "Error", $"CompanyID: {id}",Helper.GetErrorMessage(result.Return));
                    return BadRequest(new { Message =Helper.GetErrorMessage(result.Return)});
               }
               logger.LogInformation(eventId, "Success", $"CompanyID: {id}");
               return Ok(result.Object);
          }
     }
}