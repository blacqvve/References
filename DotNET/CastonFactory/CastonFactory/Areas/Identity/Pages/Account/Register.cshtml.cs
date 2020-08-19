using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CastonFactory.Data.Constants;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CastonFactory.Areas.Identity.Pages.Account
{
   [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Kullanıcı Adı")]
            [StringLength(14, ErrorMessage = "{0} en az {2} ve en fazla {1} uzunlukta olmalı", MinimumLength = 4)]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} en az  {2} ve en fazla {1} uzunlukta olmalı.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} en az  {2} ve en fazla {1} uzunlukta olmalı.", MinimumLength = 8)]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Telefon Numaranız")]
            public string PhoneNumber { get; set; }


            [DataType(DataType.Password)]
            [Display(Name = "Şifre Tekrar")]
            [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.EmailAddress)]
            [Display(Name = "E-Posta Adresi")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Ad")]
            public string Name { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Soyad")]
            public string Surname { get; set; }

            [Display(Name = "Beni Hatırla?")]
            public bool RememberMe { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/User/ContentIndex");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    Name = Input.Name,
                    PhoneNumber = Input.PhoneNumber,
                    Surname = Input.Surname
                };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var roleresult = await _userManager.AddToRoleAsync(user, RoleConstants.CONTENTPRODUCER);
                    
                    if (roleresult.Succeeded)
                    {
               
                        _logger.LogInformation("User created a new account with password.");
                        returnUrl = Url.Content("~/User/ContentIndex");
                        ViewData["Success"] = "İşlem Başarılı";
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }


                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    ViewData["Success"] = null;
                }
               return Page();
            }
            ViewData["Success"] = "Bir Hata Oluştu.";
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
