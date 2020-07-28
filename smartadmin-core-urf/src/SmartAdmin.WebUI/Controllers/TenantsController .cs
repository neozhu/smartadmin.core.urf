using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class TenantsController : Controller
  {
    private readonly ILogger<TenantsController> _logger;

    private readonly ApplicationDbContext _dbContext;
    public TenantsController(
              ILogger<TenantsController> logger,
             
              ApplicationDbContext dbContext


                               ) {
      _logger = logger;
      _dbContext = dbContext;
    }
     
 

    //租户管理
    public IActionResult Index() => View();

    //获取租户数据
    public async Task<IActionResult> GetTenantData()
    {
      var data = await this._dbContext.Tenants.ToListAsync();
      return Json(data);
    }

    
    //保存租户信息
    public async Task<JsonResult> SaveTenantData(Tenant[] tenant) {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in tenant)
          {
            if(item.TrackingState== 1)
            {
              this._dbContext.Tenants.Add(item);
            }
            if (item.TrackingState == 2)
            {
              var update = await this._dbContext.Tenants.Where(x => x.Id == item.Id).FirstAsync();
              update.Name = item.Name;
              update.Description = item.Description;
              update.ConnectionStrings = item.ConnectionStrings;
              update.Disabled = item.Disabled;
              this._dbContext.Update(update);
             
            }
            if (item.TrackingState == 3)
            {
              var delete = await this._dbContext.Tenants.Where(x => x.Id == item.Id).FirstAsync();
              this._dbContext.Tenants.Remove(delete);

            }
           
          }
          await this._dbContext.SaveChangesAsync();
          return Json(new { success = true });
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
    //删除租户信息
    public async Task<JsonResult> DeleteCheckedTenant(int[] id) {
      var items = this._dbContext.Tenants.Where(x => id.Contains(x.Id));
      foreach (var item in items)
      {
        this._dbContext.Tenants.Remove(item);
      }
      await this._dbContext.SaveChangesAsync();
      return Json(new { success = true });
    }
   
    //获取租户数据
    [HttpGet]
    public async Task<JsonResult> GetTenantData(int page = 1, int rows = 10, string sort = "Id", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<Tenant>(filterRules);
      var totalCount = 0;

      var tenants = this._dbContext.Tenants.Where(filters).OrderByName(sort, order);
       
      totalCount =await tenants.CountAsync();
      var pagerows =(await tenants.Skip(( page - 1 ) * rows).Take(rows).ToListAsync())
            .Select(n => new
      {
        Id = n.Id,
        Name = n.Name,
        n.Description,
        ConnectionStrings = n.ConnectionStrings
      }).ToList();
      var pagelist = new { total = totalCount, rows = pagerows };
      return this.Json(pagelist);
    }
     
    
    


  }
}
