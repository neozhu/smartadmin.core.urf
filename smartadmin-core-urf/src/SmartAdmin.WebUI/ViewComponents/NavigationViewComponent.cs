using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartAdmin.Dto;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Data.Models;
using SmartAdmin.WebUI.Models;

namespace SmartAdmin.WebUI.ViewComponents
{
  public class NavigationViewComponent : ViewComponent
  {
    private readonly IRoleMenuService _menuService;
    private readonly UserManager<ApplicationUser> _userManager;
    public NavigationViewComponent(
      UserManager<ApplicationUser> userManager,
      IRoleMenuService menuService)
    {
      _menuService = menuService;
      _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
      var userName = this.User.Identity.Name;
      var user = await _userManager.FindByNameAsync(userName);
      var roles = await this._userManager.GetRolesAsync(user);
      var items = await _menuService.NavDataSource(roles.ToArray()); //NavigationModel.Full;
      return View(new SmartNavigation(items));
    }
  }
}
