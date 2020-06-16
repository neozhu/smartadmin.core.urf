using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Controllers
{
  public class RoleManageController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleManageController(UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager
      )
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
      var roles = await _roleManager.Roles.OrderBy(x=>x.Name).ToListAsync();
      var users = await _userManager.Users.OrderBy(x => x.UserName).ToListAsync();
      var userinroles = new List<UserInRolesViewModel>();
      foreach (var user in users) {
        var inroles = await _userManager.GetRolesAsync(user);
        userinroles.Add(new UserInRolesViewModel()
        {
           GivenName=user.GivenName,
            Roles=inroles.ToArray(),
             UserId=user.Id,
              UserName=user.UserName
        });
      }
      ViewBag.roles = roles;
      ViewBag.userinroles = userinroles;

      return View();
    }
  }
}
