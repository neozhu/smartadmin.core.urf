using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class MenuItemsController : Controller
  {
    private readonly IMenuItemService menuItemService;
    private readonly IUnitOfWork unitOfWork;
    public MenuItemsController(IMenuItemService menuItemService,
      IUnitOfWork unitOfWork)
    {
      this.menuItemService = menuItemService;
      this.unitOfWork = unitOfWork;
    }
    //GET: MenuItems/Index
    //[OutputCache(Duration = 360, VaryByParam = "none")]
    //[Route("Index", Name = "导航菜单管理", Order = 1)]
    public ActionResult Index()=>View();
 
    //Get :MenuItems/GetData
    //For Index View datagrid datasource url
    [HttpGet]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<MenuItem>(filterRules);
      var total = await this.menuItemService
                        .Query(filters).CountAsync();
      //int pagenum = offset / limit +1;
      var pagerows = ( await menuItemService
                                 .Query(filters).Include(m => m.Parent)
                                 .OrderBy(n => n.OrderBy(sort, order))
                                 .Skip(page - 1).Take(rows)
                                 .SelectAsync())
                                 .Select(n => new
                                 {
                                   ParentTitle = ( n.Parent == null ? "" : n.Parent.Title ),
                                   Id = n.Id,
                                   Title = n.Title,
                                   Description = n.Description,
                                   LineNum = n.LineNum,
                                   Url = n.Url,
                                   Controller = n.Controller,
                                   Action = n.Action,
                                   Icon = n.Icon,
                                   n.Roles,
                                   n.Target,
                                   IsEnabled = n.IsEnabled,
                                   ParentId = n.ParentId
                                 }).ToList();
      var pagelist = new { total = total, rows = pagerows };
      return Json(pagelist);
    }
    [HttpGet]
    public async Task<JsonResult> GetDataByParentId(int parentid, int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<MenuItem>(filterRules);
      var total = await this.menuItemService
                        .Query(filters).CountAsync();
      var totalCount = 0;
      var pagerows = ( await menuItemService
                 .Queryable()
                 .Where(x=>x.ParentId== parentid)
                 .Where(filters).Include(y => y.Parent)
                 .OrderBy(sort, order)
                 .Skip(page - 1).Take(rows)
                 .ToListAsync())
                 .Select(n => new
                 {
                   ParentTitle = ( n.Parent == null ? "" : n.Parent.Title ),
                   Id = n.Id,
                   Title = n.Title,
                   Description = n.Description,
                   LineNum = n.LineNum,
                   Url = n.Url,
                   Controller = n.Controller,
                   Action = n.Action,
                   Icon = n.Icon,
                   IsEnabled = n.IsEnabled,
                   ParentId = n.ParentId
                 }).ToList();
      var pagelist = new { total = totalCount, rows = pagerows };
      return Json(pagelist);
    }
    //easyui datagrid post acceptChanges 
    [HttpPost]
    public async Task<JsonResult> AcceptChanges(MenuItem[] menuitems)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in menuitems)
          {
            this.menuItemService.ApplyChanges(item);
          }
          var result = await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true, result });
        }
        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
      }

    }
    //[OutputCache(Duration = 360, VaryByParam = "none")]
    public async Task<JsonResult> GetMenuItems(string q = "")
    {
      var rows = await this.menuItemService
                      .Queryable()
                      .Where(n => n.Title.Contains(q) && n.Controller == "#")
                      .OrderBy(n => n.Title)
                      .Select(n => new { Id = n.Id, Title = n.Title })
                      .ToListAsync();
      return Json(rows);
    }
     
    
    //Get : MenuItems/GetSubMenusByParentId/:id
    [HttpGet]
    public async Task<JsonResult> GetSubMenusByParentId(int id)
    {
      var submenus = await menuItemService.GetByParentId(id);
     
      var rows = submenus.Select(n => new
      {

        ParentTitle = ( n.Parent == null ? "" : n.Parent.Title ),
        Id = n.Id,
        Title = n.Title,
        Description = n.Description,
        LineNum = n.LineNum,
        Url = n.Url,
        Controller = n.Controller,
        Action = n.Action,
        Icon = n.Icon,
        IsEnabled = n.IsEnabled,
        ParentId = n.ParentId
      });
      return Json(rows);

    }
    [HttpPost]
    public async Task<ActionResult> CreateWithController()
    {
      try
      {
        await this.menuItemService.CreateWithControllerAsync();
        await this.unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
     
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }
    }

    //删除选中的记录
    [HttpPost]
    public async Task<JsonResult> DeleteChecked(int[] id)
    {
 
      try
      {
        foreach (var key in id) {
        await   this.menuItemService.DeleteAsync(key);
        }
        await this.unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }

    }
    //导出Excel
    [HttpPost]
    public async Task<ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "menuitems_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await menuItemService.ExportExcelAsync(filterRules, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);
    }
  

  }
}
