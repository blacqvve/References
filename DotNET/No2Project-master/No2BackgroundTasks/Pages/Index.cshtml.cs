using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using No2BackgroundTasks.Data;
using No2BackgroundTasks.Services;

namespace No2BackgroundTasks.Pages
{
    #region snippet1
    public class IndexModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IndexModel(
            ILogger<IndexModel> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

    #endregion

        public async Task OnGetAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<APIContext>();
                var logs = await context.Logs.ToListAsync();
                ViewData["logs"] = logs;
            }
        }

        #region snippet2
        public IActionResult OnPostAddTaskAsync()
        {
            return RedirectToPage();
        }
        #endregion
    }
}
