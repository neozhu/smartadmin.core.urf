using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Northwind.Data.Models;
using SmartAdmin.Entity.Models;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;
using SmartAdmin.WebUI.Models;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF;
using URF.Core.EF.Trackable;

namespace SmartAdmin.WebUI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<SmartSettings>(Configuration.GetSection(SmartSettings.SectionName));

      // Note: This line is for demonstration purposes only, I would not recommend using this as a shorthand approach for accessing settings
      // While having to type '.Value' everywhere is driving me nuts (>_<), using this method means reloaded appSettings.json from disk will not work
      services.AddSingleton(s => s.GetRequiredService<IOptions<SmartSettings>>().Value);

      services.Configure<CookiePolicyOptions>(options =>
      {
              // This lambda determines whether user consent for non-essential cookies is needed for a given request.
              options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });
      var connectionString = Configuration.GetConnectionString(nameof(SmartDbContext));

      services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
      services.AddDbContext<SmartDbContext>(options => options.UseSqlServer(connectionString));
      services.AddIdentity<ApplicationUser, IdentityRole>(options => {
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
      })
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();



      services.AddScoped<DbContext, SmartDbContext>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<ITrackableRepository<Company>, TrackableRepository<Company>>();
      services.AddScoped<ICompanyService, CompanyService>();

      services.AddTransient<IEmailSender, EmailSender>();

      services
          .AddControllersWithViews();

      services.AddRazorPages();

      services.ConfigureApplicationCookie(options =>
      {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Page/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  "default",
                  "{controller=Home}/{action=Index}");
        endpoints.MapRazorPages();
      });
    }
  }
}
