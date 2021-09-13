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

      var administrator = new ApplicationUser { UserName = "Admin", GivenName="系统管理员",  Email = "new163@163.com", EmailConfirmed = true, AvatarUrl = $"Admin.png" };
      var demo = new ApplicationUser { UserName = "Demo", GivenName = "演示账号",Email = "neozhu@126.com", EmailConfirmed = true, AvatarUrl = $"Demo.png" };

      if (userManager.Users.All(u => u.UserName != administrator.UserName))
      {
        await userManager.CreateAsync(administrator, "123456");
        await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        await userManager.CreateAsync(demo, "123456");
        await userManager.AddToRolesAsync(demo, new[] { userRole.Name });
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", administrator.TenantId.ToString()));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, administrator.UserName));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, administrator.GivenName ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantname", administrator.TenantName ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantdb", administrator.TenantDb ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, administrator.Email));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatarurl", administrator.AvatarUrl ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, administrator.PhoneNumber ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.HomePhone, administrator.PhoneNumber ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.OtherPhone, administrator.PhoneNumber ?? ""));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Locality, "zh-cn"));
        await userManager.AddClaimAsync(administrator, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Country, "china"));

        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", demo.TenantId.ToString()));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, demo.UserName));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, demo.GivenName ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantname", demo.TenantName ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantdb", demo.TenantDb ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, demo.Email));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatarurl", demo.AvatarUrl ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, demo.PhoneNumber ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.HomePhone, demo.PhoneNumber ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.OtherPhone, demo.PhoneNumber ?? ""));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Locality, "zh-cn"));
        await userManager.AddClaimAsync(demo, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Country, "china"));

      }

    }
  

   
  }
}
