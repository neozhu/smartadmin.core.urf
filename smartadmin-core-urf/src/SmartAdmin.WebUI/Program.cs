using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using SmartAdmin.Infrastructure.Persistence;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI
{
  public class Program
  {
    public async static Task Main(string[] args)
    {
    
      var host = CreateHostBuilder(args).Build();

      //Use the EF Core DB Context Service to automatically migrate database changes
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<SmartDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
          context.Database.Migrate();
          await SmartDbContextSeed.SeedSampleDataAsync(context);
        }

        var identitycontext = services.GetRequiredService<ApplicationDbContext>();
        if (identitycontext.Database.GetPendingMigrations().Any())
        {
          identitycontext.Database.Migrate();
          var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
          var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
         await  ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager); 
        }
      }

      await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                  .ConfigureLogging(logging =>
                    {
                      logging.ClearProviders();
                      logging.SetMinimumLevel(LogLevel.Trace);
                    })
                  .UseNLog();
            });
  }
}
