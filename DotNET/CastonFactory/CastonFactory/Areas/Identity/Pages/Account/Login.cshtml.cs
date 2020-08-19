using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CastonFactory.Data.Models;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using CastonFactory.Data.Constants;
using OneSignal.API;

namespace CastonFactory.Areas.Identity.Pages.Account
{
     [AllowAnonymous]
     public class LoginModel : PageModel
     {
          private readonly UserManager<User> _userManager;
          private readonly SignInManager<User> _signInManager;
          private readonly ILogger<LoginModel> _logger;


          public LoginModel(
                 SignInManager<User> signInManager,
                ILogger<LoginModel> logger,
                UserManager<User> userManager)
          {

               _userManager = userManager;
               _signInManager = signInManager;
               _logger = logger;
          }

          [BindProperty]
          public InputModel Input { get; set; }

          public IList<AuthenticationScheme> ExternalLogins { get; set; }

          public string ReturnUrl { get; set; }

          [TempData]
          public string ErrorMessage { get; set; }

          public class InputModel
          {
               [Required(ErrorMessage = "Lütfen kullanıcı adı giriniz.")]
               [DataType(DataType.Text)]
               [Display(Name = "Kullanıcı Adı")]
               public string UserName { get; set; }

               [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
               [DataType(DataType.Password)]
               [Display(Name = "Şifre")]
               public string Password { get; set; }

               [Display(Name = "Beni Hatırla?")]
               public bool RememberMe { get; set; }
          }

          public async Task OnGetAsync(string returnUrl = null)
          {
               if (!string.IsNullOrEmpty(ErrorMessage))
               {
                    ModelState.AddModelError(string.Empty, ErrorMessage);
               }

               returnUrl = returnUrl ?? Url.Content("~/");

               // Clear the existing external cookie to ensure a clean login process
               await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

               ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

               ReturnUrl = returnUrl;
          }

          public async Task<IActionResult> OnPostAsync(string returnUrl = null)
          {

               returnUrl = returnUrl ?? Url.Content("~/");

               if (ModelState.IsValid)
               {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    var user = await _signInManager.UserManager.FindByNameAsync(Input.UserName);
                    if (user == null)
                    {
                         ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı. Lütfen kullanıcı adınızı kontrol edin.");
                         return Page();
                    }     
                         IList<string> role = await _signInManager.UserManager.GetRolesAsync(user);
                    foreach (var item in role)
                    {
                         switch (item)
                         {
                              case RoleConstants.CONTENTPRODUCER:
                                   returnUrl = Url.Content("~/User/ContentIndex");
                                   break;
                              case RoleConstants.ADMIN:
                                   returnUrl = Url.Content("~/Admin/Index");
                                   break;
                              case RoleConstants.MANAGER:
                                   returnUrl = Url.Content("~/Home/Index");
                                   break;
                         }
                    }
                    if (result.Succeeded)
                    {


                         _logger.LogWarning($"User logged in. {Input.UserName}");
                         if (role.Contains(RoleConstants.MANAGER))
                         {
                              Signal signal = new Signal();
                              var resp = await signal.CreateNotification(new string[] { "AdminSegment" }, $"{user.FullName} isimli otoriter giriş yaptı.", Templates.Manager_Login, Configuration.ApplicationID);
                              _logger.LogWarning(await resp.Content.ReadAsStringAsync());
                         }

                         return LocalRedirect(returnUrl);

                    }
                    if (result.RequiresTwoFactor)
                    {
                         return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }

                    if (result.IsLockedOut)
                    {
                         _logger.LogWarning("User account locked out.");
                         return RedirectToPage("./Lockout");
                    }
                    else
                    {
                         ModelState.AddModelError(string.Empty, "Başarısız giriş denemesi..");
                         return Page();
                    }
               }

               // If we got this far, something failed, redisplay form
               return Page();
          }
     }
}
