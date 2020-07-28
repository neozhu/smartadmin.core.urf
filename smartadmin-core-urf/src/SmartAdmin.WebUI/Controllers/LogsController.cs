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
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;

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

    private readonly ILogger<LogsController> logger;
    private readonly SqlSugar.ISqlSugarClient db;
    private readonly ApplicationDbContext dbContext;
    public LogsController(
      ApplicationDbContext dbContext,
      SqlSugar.ISqlSugarClient db,
      ILogger<LogsController> logger
      )
    {
      this.db = db;
      this.logger = logger;
      this.dbContext = dbContext;
    }
    //GET: Logs/Index
    //[OutputCache(Duration = 60, VaryByParam = "none")]
    public ActionResult Index()=>  View();
  

    //Get :Logs/GetData
    //For Index View datagrid datasource url
    //更新日志状态
    [HttpGet]
    public async Task<IActionResult> SetLogState(int id)
    {
      var log = await this.dbContext.Logs.FindAsync(id);
      log.Resolved = true;
      await this.dbContext.SaveChangesAsync();
      return Json(new { success=true});
    }
    [HttpGet]
    public async Task<IActionResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<Log>(filterRules);
      var total = await this.dbContext.Logs
                        .Where(filters)
                        .AsNoTracking()
                        .CountAsync();
      var pagerows = await this.dbContext
                                 .Logs
                                 .Where(filters)
                                 .OrderBy(sort, order)
                                 .Skip(page - 1).Take(rows)
                                 .AsNoTracking()
                                 .ToListAsync();
                                 
      var pagelist = new { total = total, rows = pagerows };
      return this.Json(pagelist);
    }
    //easyui datagrid post acceptChanges 
    public async Task<IActionResult> GetChartData()
    {
      var levels = new string[] { "Info", "Trace", "Debug", "Warn", "Error", "Fatal" };
      var sql = @"SELECT [level],
       CONVERT(Datetime,format(min(Logged),'yyyy-MM-dd HH:00:00')) AS [time],
       COUNT(*) AS total
FROM AspNetLogs
where DATEDIFF(D, GETDATE(), Logged)> -3
GROUP BY [level], CAST(Logged as date),
       DATEPART(hour, Logged)
order by [level], CAST(Logged as date),
       DATEPART(hour, Logged)";
      var data = await this.db.Ado.SqlQueryAsync<logtimesummary>(sql);
      var date = DateTime.Now.AddDays(-2).Date;
      var today = DateTime.Now.AddDays(1).Date;
      var list = new List<dynamic>();
      while ((date = date.AddHours(1)) < today)
      {
        foreach (var level in levels)
        {
          var item = data.Where(x => x.time == date && x.level == level).FirstOrDefault();
          if (item != null)
          {
            list.Add(new { time = date.ToString("yyyy-MM-dd HH:mm"), level = level, total = item.total });
          }
          else
          {
            list.Add(new { time = date.ToString("yyyy-MM-dd HH:mm"), level = level, total = 0 });

          }
        }

      }
      var sql1 = @"select Level [level],count(*) total
FROM AspNetLogs
where DATEDIFF(D, GETDATE(), Logged)> -3
group by Level";
      var array = await this.db.Ado.SqlQueryAsync<loglevelsummary>(sql1);
      
      var group = new List<dynamic>();
      foreach (var level in levels)
      {
        var item = array.Where(x => x.level == level).FirstOrDefault();
        if (item != null)
        {
          group.Add(new { level, item.total });
        }
        else
        {
          group.Add(new { level, total = 0 });

        }
      }
      return Json(new { list = list, group = group });
    }
  }
}
