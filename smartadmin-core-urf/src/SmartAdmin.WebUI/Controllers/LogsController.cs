using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace SmartAdmin.WebUI.Controllers
{
  /// <summary>
  /// File: LogsController.cs
  /// Purpose:系统管理/系统日志
  /// Created Date: 9/19/2019 8:51:53 AM
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// TODO: Registers the type mappings with the Unity container(Mvc.UnityConfig.cs)
  /// <![CDATA[
  ///    container.RegisterType<IRepositoryAsync<Log>, Repository<Log>>();
  ///    container.RegisterType<ILogService, LogService>();
  /// ]]>
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  [Authorize]
  public class LogsController : Controller
  {
    private readonly ILogService logService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<LogsController> logger;
    public LogsController(
      ILogService logService,
      IUnitOfWork unitOfWork,
      ILogger<LogsController> logger
      )
    {
      this.logService = logService;
      this.unitOfWork = unitOfWork;
      this.logger = logger;
    }
    //GET: Logs/Index
    //[OutputCache(Duration = 60, VaryByParam = "none")]
    public async Task<ActionResult> Index()
    {




      return View();
    }

    //Get :Logs/GetData
    //For Index View datagrid datasource url
    //更新日志状态
    [HttpGet]
    public async Task<IActionResult> SetLogState(int id)
    {

      await this.logService.Resolved(id);
      await this.unitOfWork.SaveChangesAsync();
      return Json(new { success = true });
    }
    [HttpGet]
    public ActionResult Notify() => this.PartialView("_notifications");
    [HttpGet]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<Log>(filterRules);
      var total = await this.logService
                        .Query(filters).CountAsync();
      var pagerows = ( await this.logService
                                 .Query(filters)
                                 .OrderBy(n => n.OrderBy(sort, order))
                                  .Skip(page - 1).Take(rows).SelectAsync())
                                 .Select(n => new
                                 {

                                   Id = n.Id,
                                   MachineName = n.MachineName,
                                   Logged = n.Logged?.ToString("yyyy-MM-dd HH:mm:ss"),
                                   Level = n.Level,
                                   RequestIp = n.RequestIp,
                                   Message = n.Message,
                                   Exception = n.Exception,
                                   Properties = n.Properties,
                                   Identity = n.Identity,
                                   Logger = n.Logger,
                                   Callsite = n.Callsite,
                                   n.RequestForm,
                                   n.SiteName,
                                   Resolved = n.Resolved
                                 }).ToList();
      var pagelist = new { total = total, rows = pagerows };
      return this.Json(pagelist);
    }
    //easyui datagrid post acceptChanges 
    public async Task<JsonResult> GetChartData()
    {
      var data = await this.logService.GetSummaryData();
      return Json(new { list = data.Item1, group = data.Item2 });
    }
  }
}
