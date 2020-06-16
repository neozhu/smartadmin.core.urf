using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Extensions;
using URF.Core.Abstractions;
using URF.Core.EF;
namespace SmartAdmin.WebUI.Controllers
{
  /// <summary>
  /// File: NotificationsController.cs
  /// Purpose:系统管理/消息推送
  /// Created Date: 9/16/2019 10:51:44 AM
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// TODO: Registers the type mappings with the Unity container(Mvc.UnityConfig.cs)
  /// <![CDATA[
  ///    container.RegisterType<IRepositoryAsync<Notification>, Repository<Notification>>();
  ///    container.RegisterType<INotificationService, NotificationService>();
  /// ]]>
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  [Authorize]

  public class NotificationsController : Controller
  {
    private readonly INotificationService notificationService;
    private readonly IUnitOfWork unitOfWork;
    //private readonly IHubContext hub;
    private readonly ILogger<CompaniesController> logger;
    public NotificationsController(
      INotificationService notificationService,
      ILogger<CompaniesController> logger,
      IUnitOfWork unitOfWork)
    {
      
      this.notificationService = notificationService;
      this.unitOfWork = unitOfWork;
      this.logger = logger;
      //this.hub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
    }
    //获取未读的消息
    [HttpGet]
    public async Task<JsonResult> GetNotifyData(string userName = "",string notifygroup="")
    {
      userName = string.IsNullOrEmpty(userName) ? this.User.Identity.Name : userName;
      
        var data = await this.notificationService.Queryable()
          .Where(x => x.Read == false &&
          x.Group.Contains(notifygroup) &&
          ( x.To == "ALL" || x.To == userName ))
          .OrderByDescending(x => x.Id)
          .ToListAsync();
        return Json(new { data = data } );
      
    }
    [HttpGet]
    public async Task<JsonResult> SetRead(int id) {
      //await this.notificationService.Queryable().Where(x => x.Id == id).UpdateAsync(x => new Notification() { Read = true });
      //this.hub.Clients.All.broadcastChanged();
      return Json(new { success = true });
    }
    [HttpGet]
    public async Task<JsonResult> SetAllRead(string userName)
    {
      //await this.notificationService.Queryable().Where(x => x.To == userName || x.To=="ALL").UpdateAsync(x => new Notification() { Read = true });
      //this.hub.Clients.All.broadcastChanged();
      return Json(new { success = true });
    }
    //GET: Notifications/Index
    //[OutputCache(Duration = 60, VaryByParam = "none")]
    [Route("Index", Name = "消息推送", Order = 1)]
    public ActionResult Index() => this.View();

    //Get :Notifications/GetData
    //For Index View datagrid datasource url
    [HttpGet]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<Notification>(filterRules);
      var total = await this.notificationService
                           .Query(filters).CountAsync();
      var pagerows = ( await this.notificationService
                                 .Query(filters)
                                 .OrderBy(n => n.OrderBy(sort, order))
                                 .Skip(page - 1).Take(rows).SelectAsync())
                                 .Select(n => new
                                 {

                                   Id = n.Id,
                                   Title = n.Title,
                                   Content = n.Content,
                                   Link = n.Link,
                                   Read = n.Read,
                                   From = n.From,
                                   To = n.To,
                                   Group = n.Group,
                                   PublishDate = n.PublishDate.ToString("yyyy-MM-dd HH:mm:ss")
                                 }).ToList();
      var pagelist = new { total = total, rows = pagerows };
      return this.Json(pagelist);
    }
    //easyui datagrid post acceptChanges 
    [HttpPost]
    public async Task<JsonResult> AcceptChanges(Notification[] notifications)
    {
 
      if (this.ModelState.IsValid)
      {
        try
        {
          //var hub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
          foreach (var item in notifications)
          {
            this.notificationService.ApplyChanges(item);
            
          }
           await this.unitOfWork.SaveChangesAsync();
          //this.hub.Clients.All.broadcastChanged();
          return this.Json(new { success = true });
        }
        
        catch (Exception e)
        {
          return this.Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return this.Json(new { success = false, err = modelStateErrors });
      }

    }
    //删除选中的记录
    [HttpPost]
    public async Task<JsonResult> DeleteChecked(int[] id)
    {

      try
      {
        await this.notificationService.Delete(id);
        await this.unitOfWork.SaveChangesAsync();
        return this.Json(new { success = true } );
      }
      
      catch (Exception e)
      {
        return this.Json(new { success = false, err = e.GetBaseException().Message } );
      }
    }
    //导出Excel
    [HttpPost]
    public async Task< ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "notifications_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await this.notificationService.ExportExcelAsync(filterRules, sort, order);
      return this.File(stream, "application/vnd.ms-excel", fileName);
    }
    

  }
}
