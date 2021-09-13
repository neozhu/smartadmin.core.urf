using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SmartAdmin.WebUI.Data.Models;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Data
{
  public static class ApplicationDbContextSeed
  {
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      var administratorRole = new IdentityRole("Admin");
      var userRole = new IdentityRole("Basic");

      if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
      {
        await roleManager.CreateAsync(administratorRole);
        await roleManager.CreateAsync(userRole);
        
      }

      var administrator = new ApplicationUser { UserName = "Admin", GivenName="系统管理员",  Email = "new163@163.com", EmailConfirmed = true, AvatarUrl = $"https://cdn.v2ex.com/gravatar/{"new163@163.com".ToMD5()}?s=120&d=retro" };
      var demo = new ApplicationUser { UserName = "Demo", GivenName = "演示账号",Email = "neozhu@126.com", EmailConfirmed = true, AvatarUrl = $"https://cdn.v2ex.com/gravatar/{"neozhu@126.com".ToMD5()}?s=120&d=retro" };

      if (userManager.Users.All(u => u.UserName != administrator.UserName))
      {
        await userManager.CreateAsync(administrator, "123456");
        await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        await userManager.CreateAsync(demo, "123456");
        await userManager.AddToRolesAsync(demo, new[] { userRole.Name });
      }

    }
  

   
  }
}
