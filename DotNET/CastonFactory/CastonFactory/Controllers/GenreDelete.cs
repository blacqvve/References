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
    public class GenreDelete : Controller
    {
        private readonly DataContext context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<GenreDelete> _logger;
        private readonly IContentManager contentManager;
        private readonly IWebHostEnvironment hosting;
        private readonly IHelpers helpers;

        private Signal signal;

        public GenreDelete(ILogger<GenreDelete> logger, UserManager<User> userManager,
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
        public async Task<IActionResult> GenreIndex()
        {
            var genre = await context.Genres.ToListAsync();


            return View(genre);
        }

        public async Task<IActionResult> DeleteGenre(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thmes = await context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thmes == null)
            {
                return NotFound();
            }

            return View(thmes);
        }


        [HttpPost, ActionName("GenreDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenreDeleteConfirmed(Guid id)
        {
            var genre = await context.Genres.FindAsync(id);
            var nullgenre = await context.Genres.FirstAsync(x => x.Id.ToString() == "012c55fc-4172-4b36-8263-8db6823ea7d5");
            var contents = await context.Contents.Where(x => x.Genre.Id == genre.Id).ToListAsync();
            if (contents.Count == 0)
            {
                context.Genres.Remove(genre);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Genres.Remove(genre);
                foreach (var item in contents)
                {


                    item.Genre = nullgenre;



                }
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(GenreIndex));
        }
    }
}
