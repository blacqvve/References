using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CastonFactory.Data;
using CastonFactory.Data.Constants;
using CastonFactory.Data.Data;
using CastonFactory.Data.Managers;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CastonFactory.Controllers
{
    [AllowAnonymous]
    public class RouteController : Controller
    {
        private readonly DataContext _context;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public RouteController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<UserController> logger, DataContext context, IHelpers _helpers)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }



        public async Task<IActionResult> IndexAsync(string returnUrl = null)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
    
            if (user != null)
            {
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
            }
            else
            {
                returnUrl = Url.Content("~/Identity/Account/Login");
            }


            return Redirect(returnUrl);
        }
    }
}
