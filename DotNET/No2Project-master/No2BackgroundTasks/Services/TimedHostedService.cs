using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using No2API.Entities.Models;
using No2BackgroundTasks.Data;

namespace No2BackgroundTasks.Services
{
    #region snippet1
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private List<Timer> timers = new List<Timer>();

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            timers.Add(new Timer(CheckSubscriptions, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5)));

            timers.Add(new Timer(ResetViews, null, TimeSpan.Zero,
                TimeSpan.FromDays(1)));

            return Task.CompletedTask;
        }

        private void CheckSubscriptions(object state)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<APIContext>();

                _logger.LogInformation("Checking user subscriptions...");
                var users = context.Users
                     .Include(x=>x.Orders)
                     .Where(x => x.HasPaid == true)
                     .Where(x => x.Orders.Any(order => order.OrderId.StartsWith("MANUALORDER_")))
                     .ToList();
                _logger.LogInformation($"User list: {string.Join(",", users.Select(k=>k.Email))}");
                foreach (var user in users)
                {

                    if (user.ActivationDate >= DateTime.MinValue && user.ExpirationDate <= DateTime.Now)
                    {
                        var msg = $"User (id: {user.Id}, email: {user.Email})'s subscription has ended on {DateTime.Now} \n" +
                            $"User's expiration date ({user.ExpirationDate}) and activation date({user.ActivationDate}) are resetting... ";
                        _logger.LogInformation(msg);
                        context.Logs.Add(new Log
                        {
                            LogLevel = "[INFO]",
                            CategoryName = "User subscription correction",
                            Msg = msg,
                            User = user.Id.ToString(),
                            Timestamp = DateTime.Now
                        });
                        user.HasPaid = false;
                        user.ActivationDate = DateTime.MinValue;
                        user.ExpirationDate = DateTime.MinValue;
                        var userOrders = context.Orders.Where(x => x.User.Id == user.Id).ToList();
                        if (userOrders != null)
                        {
                            context.RemoveRange(userOrders);
                        }
                        context.Update(user);
                        context.SaveChanges();
                    }

                }
            }
        }

        private void ResetViews (object state)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<APIContext>();

                _logger.LogInformation("Checking user views...");
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    user.Views = 0;                   
                    context.Update(user);                   
                    _logger.LogInformation($"User views reset at {DateTime.Now}");
                }
                context.Logs.Add(new Log
                {
                    LogLevel = "[INFO]",
                    CategoryName = "User view reset",
                    Msg = $"User views reset",
                    User = "",
                    Timestamp = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            foreach (var timer in timers)
            {
                timer?.Change(Timeout.Infinite, 0);
            }          
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            foreach (var timer in timers)
            {
                timer?.Dispose();
            }
        }
    }
    #endregion
}
