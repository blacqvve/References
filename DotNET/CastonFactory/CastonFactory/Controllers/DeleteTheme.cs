using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CastonFactory.Data;
using CastonFactory.Data.Data;
using CastonFactory.Data.Managers;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneSignal.API;

namespace CastonFactory.Controllers
{
    [Authorize(Roles = ("Admin"))]
    public class DeleteTheme : Controller
    {
        private readonly DataContext context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DeleteTheme> _logger;
        private readonly IContentManager contentManager;
        private readonly IWebHostEnvironment hosting;
        private readonly IHelpers helpers;
   
        private Signal signal;

        public DeleteTheme(ILogger<DeleteTheme> logger, UserManager<User> userManager,
            SignInManager<User> signInManager, DataContext _context, IContentManager _contentManager, [FromServices] IWebHostEnvironment hosting, IHelpers _helpers)
        {
            this.context = _context;
            contentManager = _contentManager;
            helpers = _helpers;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            this.hosting = hosting;

            signal = new Signal();
        }

        public async Task<IActionResult> ThemeIndex()
        {
            var theme = await context.Themes.ToListAsync();


            return View(theme);
        }


    



        public async Task<IActionResult> ThemeDelete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thmes = await context.Themes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (thmes == null)
            {
                return NotFound();
            }

            return View(thmes);
        }


        [HttpPost, ActionName("ThemeDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemeDeleteConfirmed(Guid id)
        {
            var theme = await context.Themes.FindAsync(id);
            var nulltheme = await context.Themes.FirstAsync(x => x.Id.ToString() == "81613883-b062-49ba-83f4-68aebce0071a");
            var contents = await context.Contents.Where(x => x.Theme.Id == theme.Id).ToListAsync();
            if (contents.Count == 0)
            {
                context.Themes.Remove(theme);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Themes.Remove(theme);
                foreach (var item in contents)
                {


                    item.Theme = nulltheme;



                }
            }


  
           await context.SaveChangesAsync();
            return RedirectToAction(nameof(ThemeIndex));
        }




     


    }
}
