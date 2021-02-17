using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Dto;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Data.Models;
using URF.Core.Abstractions;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class RoleMenusController : Controller
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    private readonly IRoleMenuService _roleMenuService;
    private readonly IMenuItemService _menuItemService;
    private readonly IUnitOfWork _unitOfWork;

    public RoleMenusController(
      IRoleMenuService roleMenuService,
      IUnitOfWork unitOfWork,
      IMenuItemService menuItemService,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager
      )
    {
      _roleMenuService = roleMenuService;
      _menuItemService = menuItemService;

      _unitOfWork = unitOfWork;
      this.userManager = userManager;
      this.roleManager = roleManager;
    }

    // GET: RoleMenus/Index
    public async Task<ActionResult> Index()
    {
      var roles = await this.roleManager.Roles.ToListAsync();
      var roleview = new List<RoleView>();
      foreach (var role in roles)
      {
        var mymenus = await _roleMenuService.GetByRoleNameAsync(role.Name);
        var r = new RoleView();
        r.RoleName = role.Name;
        r.Count = mymenus.Count();
        roleview.Add(r);
      }
      ViewBag.Roles = roleview;
      return View();
    }


    public async Task<IActionResult> RenderMenus()
    {
      var user =await this.userManager.FindByNameAsync(this.User.Identity.Name);
      var roles =await this.userManager.GetRolesAsync(user);
      //var roles = new string[] { "admin" };
      var menus = _roleMenuService.RenderMenus(roles.ToArray());
      return PartialView("_navMenuBar", menus);
    }
    public async Task<ActionResult> GetMenuList()
    {
      var menus = _menuItemService.Queryable().Include(x => x.Children).Where(x => x.IsEnabled).OrderBy(y => y.LineNum);
      var totalCount = menus.Count();
      var datarows = await menus.Select(x => new
      {
        Id = x.Id,
        Title = x.Title,
        Code = x.LineNum,
        _parentId = x.ParentId,
        Url = x.Url,
        Create = true,
        Edit = true,
        Delete = true,
        Import = true,
        Export = true,
        FunctionPoint1 = false,
        FunctionPoint2 = false,
        FunctionPoint3 = false
      }).OrderBy(x => x._parentId).ThenBy(x => x.Code).ToListAsync();
      var pagelist = new { total = totalCount, rows = datarows };
      return Json(pagelist);
    }
    public async Task<ActionResult> GetMenus(string roleName)
    {

      var rolemenus = await _roleMenuService.GetByRoleNameAsync(roleName);
      //var all = _roleMenuService.RenderMenus(roleName);
      return Json(rolemenus);

    }

    [HttpPost]
    public async Task<ActionResult> Submit(RoleMenusView[] selectmenus)
    {

      await _roleMenuService.AuthorizeAsync(selectmenus);
      await _unitOfWork.SaveChangesAsync();
      return Json(new { success = true });
    }

    // Get :RoleMenus/PageList
    // For Index View Boostrap-Table load  data 
    [HttpGet]
    public async Task<IActionResult> PageList(int offset = 0, int limit = 10, string search = "", string sort = "", string order = "")
    {
   
      var total = await this._roleMenuService
                        .Query().CountAsync();
      //int pagenum = offset / limit + 1;
      var pagerows = (await this._roleMenuService
        .Query()
        .Include(r => r.MenuItem)
        .OrderBy(n => n.OrderBy($"{sort} {order}"))
        .Skip(offset).Take(limit)
        .SelectAsync())
        .Select(n =>
        new {
          MenuItemTitle = ( n.MenuItem == null ? "" : n.MenuItem.Title ),
          Id = n.Id,
          RoleName = n.RoleName,
          MenuId = n.MenuId,
          IsEnabled = n.IsEnabled
          })
        .ToList();
      var pagelist = new { total = total, rows = pagerows };
      return Json(pagelist);
    }

 





  }
}
