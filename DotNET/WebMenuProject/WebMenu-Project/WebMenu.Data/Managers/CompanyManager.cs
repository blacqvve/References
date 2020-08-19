using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using WebMenu.Data.Data;
using WebMenu.Data.Misc.Enums;
using WebMenu.Data.Misc.Structs;
using WebMenu.Data.Models;

namespace WebMenu.Data.Managers
{
     public interface ICompanyManager
     {
          #region Get Company
          /// <summary>
          /// This class gets first or default company from context from id
          /// </summary>
          /// <param name="id"></param>
          /// <returns> 
          /// Company information with CompanyLogo Image data
          /// </returns>
          public Task<Company> GetCompany(Guid id);
          /// <summary>
          /// This methods gets a referance for user related company from userId 
          /// </summary>
          /// <param name="userId"></param>
          /// <returns></returns>
          public Task<Company> GetCompanyFromUser(Guid userId);
          /// <summary>
          /// This method gets a  referance of company related to user from userEmail
          /// </summary>
          /// <param name="email"></param>
          /// <returns></returns>
          public Task<Company> GetCompanyFromUser(string email);
          /// <summary>
          /// This method returns a company list from context Ordered by CreateDate property. Can take int parameter for data lenght.
          /// </summary>
          /// <param name="count"></param>
          /// <returns></returns>
          public Task<List<Company>> GetCompanies(int? count);
          /// <summary>
          /// Returns a active company list from context.
          /// </summary>
          /// <returns></returns>
          public Task<List<Company>> GetActiveCompanies();
          #endregion

          #region Create Company
          /// <summary>
          /// This method creates a database entry for new company
          /// </summary>
          /// <param name="company"></param>
          /// <returns>
          /// Returns ErrorReturns enum type. ErrorReturns.Ok for success.
          /// </returns>
          public Task<ReturnObject<ErrorReturns, Company>> CreateCompanyAsync(Company company);
          /// <summary>
          /// This method creates a database entry for new company from Json string
          /// </summary>
          /// <param name="jsonData"></param>
          /// <returns></returns>
          public Task<ReturnObject<ErrorReturns, Company>> CreateCompanyAsync(string jsonData);
          #endregion


          #region Delete Company
          /// <summary>
          /// Deletes company from database with id
          /// </summary>
          /// <param name="id"></param>
          /// <returns></returns>
          public Task<ErrorReturns> DeleteCompany(Guid id);
          /// <summary>
          /// Deletes company from database with entity
          /// </summary>
          /// <param name="_company"></param>
          /// <returns></returns>
          public Task<ErrorReturns> DeleteCompany(Company _company);
          #endregion

          #region Update Company
          /// <summary>
          /// Updates given company in context.
          /// </summary>
          /// <param name="company"></param>
          /// <returns></returns>
          public Task<ReturnObject<ErrorReturns, Company>> UpdateCompanyAsync(Company company);
          /// <summary>
          /// Updates company from string data
          /// </summary>
          /// <param name="json"></param>
          /// <returns></returns>

          public Task<ReturnObject<ErrorReturns, Company>> UpdateCompanyAsync(string json);
          #endregion

          #region Company menu
          /// <summary>
          /// Gets company related menu with full information
          /// </summary>
          /// <param name="companyId"></param>
          /// <returns></returns>
          public Task<(ErrorReturns Return, Menu Object)> GetCompanyMenuAsync(Guid companyId);
          /// <summary>
          /// Gets company related menu with full information
          /// </summary>
          /// <param name="company"></param>
          /// <returns></returns>
          public Task<(ErrorReturns Return, Menu Object)> GetCompanyMenuAsync(Company company);
          #endregion

     }
     public class CompanyManager : ICompanyManager
     {
          private readonly APIContext context;
          public CompanyManager(APIContext _context)
          {
               context = _context;
          }

          #region Create company region
          public async Task<ReturnObject<ErrorReturns, Company>> CreateCompanyAsync(string jsonData)
          {
               var company = JsonConvert.DeserializeObject<Company>(jsonData);
               if (company == null)
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.NotFound, null, null);
               var _company = company;
               _company.CreateDate = DateTime.Now;
               _company.ModifyDate = DateTime.Now;
               _company.CompanyId = Guid.NewGuid();
               _company.Subscription = true;
               try
               {
                    await context.AddAsync(_company);
                    await context.SaveChangesAsync();
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.Ok, _company, null);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.ServerError, null, e);

               }
          }
          public async Task<ReturnObject<ErrorReturns, Company>> CreateCompanyAsync(Company company)
          {
               if (company == null)
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.NotFound, null, null);
               var _company = company;
               _company.CreateDate = DateTime.Now;
               _company.ModifyDate = DateTime.Now;
               _company.CompanyId = Guid.NewGuid();
               _company.Subscription = true;
               try
               {
                    await context.AddAsync(_company);
                    await context.SaveChangesAsync();
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.Ok, _company, null);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.ServerError, null, e);

               }
          }
          #endregion


          #region Delete company region
          public async Task<ErrorReturns> DeleteCompany(Guid id)
          {
               var company = await context.Companies
                .Include(x => x.QRPicture)
               .Include(x => x.CompanyLogo)
               .Include(x => x.Menu)
               .ThenInclude(x => x.Categories)
               .ThenInclude(x => x.MenuItems)
               .ThenInclude(x => x.Pictures)
              .Include(x=>x.Menu)
              .ThenInclude(x=>x.Categories)
              .ThenInclude(x=>x.CategoryImage)
               . FirstOrDefaultAsync(x => x.CompanyId == id);
               if (company == null)
                    return ErrorReturns.NotFound;
               try
               {
                    //foreach (var item in company.Menu.Categories)
                    //{
                    //     foreach (var t in item.MenuItems)
                    //     {
                    //          context.Pictures.RemoveRange(t.Pictures);
                    //     }
                    //     context.RemoveRange(item.MenuItems);
                    //     context.Remove(item.CategoryImage);
                    //}
                    
                    //context.Categories.RemoveRange(company.Menu.Categories);
                    //context.Pictures.Remove(company.QRPicture);
                    //context.Pictures.Remove(company.CompanyLogo);
                    context.Remove(company);
                    await context.SaveChangesAsync();
                    return ErrorReturns.Ok;
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return ErrorReturns.ServerError;
               }
          }
          public async Task<ErrorReturns> DeleteCompany(Company _company)
          {
               var company = await context.Companies

                    .FirstOrDefaultAsync(x => x.CompanyId == _company.CompanyId);
               if (company == null)
                    return ErrorReturns.NotFound;
               try
               {
                    context.Companies.Remove(company);
                    await context.SaveChangesAsync();
                    return ErrorReturns.Ok;
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return ErrorReturns.ServerError;
               }
          }

          #endregion


          #region Get company region
          public async Task<Company> GetCompanyFromUser(Guid userId)
          {
               var company = await context.Companies
                    .Include(x => x.CompanyLogo)
                    .Include(x => x.QRPicture)
                    .FirstOrDefaultAsync(x => x.User.Id == userId.ToString());
               return company;
          }
          public async Task<Company> GetCompanyFromUser(string email)
          {
               var company = await context.Companies
                    .Include(x => x.CompanyLogo)
                    .Include(x => x.QRPicture)
                    .FirstOrDefaultAsync(x => x.User.Email == email);
               return company;
          }
          public async Task<Company> GetCompany(Guid id)
          {
               return await context.Companies
                     .Include(x => x.CompanyLogo)
                     .Include(x => x.QRPicture)
                     .Include(x=>x.Menu)
                     .ThenInclude(x=>x.Categories)
                     .ThenInclude(x=>x.MenuItems)
                     .ThenInclude(x=>x.Pictures)
                     .FirstOrDefaultAsync(x => x.CompanyId == id);
          }

          public async Task<List<Company>> GetCompanies(int? count)
          {
               var _count = count ?? context.Companies.Count();
               var companies = await context.Companies.Take(_count)
                    .Include(x => x.CompanyLogo)
                    .Include(x => x.QRPicture)
                    .OrderByDescending(x => x.CreateDate)
                    .ToListAsync();
               return companies;
          }
          public async Task<List<Company>> GetActiveCompanies()
          {
               var companies = await context.Companies.Where(x => x.Subscription == true)
                    .Include(x => x.CompanyLogo)
                    .OrderByDescending(x => x.CreateDate)
                    .ToListAsync();
               return companies;
          }
          #endregion

          #region Update company region
          public async Task<ReturnObject<ErrorReturns, Company>> UpdateCompanyAsync(Company _company)
          {
               var company = await context.Companies.FirstOrDefaultAsync(x => x.CompanyId == _company.CompanyId);
               if (company == null)
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.NotFound, null, null);


               company = _company;
               company.ModifyDate = DateTime.Now;
               try
               {
                    context.Companies.Update(company);
                    await context.SaveChangesAsync();
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.Ok, company, null);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.ServerError, null, e);

               }
          }

          public async Task<ReturnObject<ErrorReturns, Company>> UpdateCompanyAsync(string json)
          {
               if (String.IsNullOrEmpty(json))
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.NotFound, null, null);
               var company = JsonConvert.DeserializeObject<Company>(json);
               company.ModifyDate = DateTime.Now;
               try
               {
                    context.Companies.Update(company);
                    await context.SaveChangesAsync();
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.Ok, company, null);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return new ReturnObject<ErrorReturns, Company>(ErrorReturns.ServerError, company, e);
               }

          }
          #endregion


          #region Company Menu Region

          public async Task<(ErrorReturns Return, Menu Object)> GetCompanyMenuAsync(Guid companyId)
          {
               var company = await context.Companies.Include(x => x.Menu).FirstOrDefaultAsync(x => x.CompanyId == companyId);
               if (company == null || company.Menu == null)
                    return (ErrorReturns.NotFound, null);
               try
               {
                    var menu = await context.Menus
                         .Include(x => x.Categories)
                         .ThenInclude(x => x.MenuItems)
                         .ThenInclude(x => x.Pictures)
                                  .FirstOrDefaultAsync(x => x.MenuId == company.Menu.MenuId);


                    return (ErrorReturns.Ok, menu);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return (ErrorReturns.ServerError, null);
               }
          }
          public async Task<(ErrorReturns Return, Menu Object)> GetCompanyMenuAsync(Company _company)
          {
               var company = await context.Companies.Include(x => x.Menu).FirstOrDefaultAsync(x => x.CompanyId == _company.CompanyId);
               if (company == null || company.Menu == null)
                    return (ErrorReturns.NotFound, null);
               try
               {
                    var menu = await context.Menus
                         .Include(x => x.Categories)
                         .ThenInclude(x => x.CategoryImage)
                         .Include(x => x.Categories)
                         .ThenInclude(x => x.MenuItems)
                         .ThenInclude(x => x.Pictures)
                                  .FirstOrDefaultAsync(x => x.MenuId == company.Menu.MenuId);

                    return (ErrorReturns.Ok, menu);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return (ErrorReturns.ServerError, null);
               }
          }
          #endregion
     }
}
