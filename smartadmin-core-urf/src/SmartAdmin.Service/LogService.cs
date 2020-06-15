using System.Threading;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using Microsoft.Extensions.Logging;
using URF.Core.EF;
using System.Collections.Generic;
using SmartAdmin.Dto;

namespace SmartAdmin.Service
{
  /// <summary>
  /// File: LogService.cs
  /// Purpose: Within the service layer, you define and implement 
  /// the service interface and the data contracts (or message types).
  /// One of the more important concepts to keep in mind is that a service
  /// should never expose details of the internal processes or 
  /// the business entities used within the application. 
  /// Created Date: 9/19/2019 8:51:51 AM
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  public class LogService : Service<Log>, ILogService
  {
    private readonly ILogger<Log> logger;
    private readonly SqlSugar.ISqlSugarClient db;
    public LogService(
      SqlSugar.ISqlSugarClient db,
      ILogger<Log> logger,
      ITrackableRepository<Log> repository) : base(repository)
    {
      this.db = db;
      this.logger = logger;
    }

      public async Task<Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>>> GetSummaryData()
    {
      var sql = @"SELECT
       CONVERT(Datetime,format(min(Logged),'yyyy-MM-dd HH:00:00')) AS [time],
       COUNT(*) AS total
FROM Logs
where DATEDIFF(D, GETDATE(), Logged)> -3
GROUP BY CAST(Logged as date),
       DATEPART(hour, Logged)
order by  CAST(Logged as date),
       DATEPART(hour, Logged)";
      var data = await this.db.Ado.SqlQueryAsync<logtimesummary>(sql);
      var date = DateTime.Now.AddDays(-2).Date;
      var today = DateTime.Now.AddDays(1).Date;
      var list = new List<dynamic>();
      while ((date = date.AddHours(1)) < today)
      {
        var item = data.Where(x => x.time == date).FirstOrDefault();
        if (item != null)
        {
          list.Add(new { time = date.ToString("yyyy-MM-dd HH:mm"), total = item.total });
        }
        else
        {
          list.Add(new { time = date.ToString("yyyy-MM-dd HH:mm"), total = 0 });

        }

      }
      var sql1 = @"select Level [level],count(*) total
FROM Logs
where DATEDIFF(D, GETDATE(), Logged)> -3
group by Level";
      var array = await this.db.Ado.SqlQueryAsync<loglevelsummary>(sql1);
      var levels = new string[] { "Info", "Trace", "Debug", "Warn", "Error", "Fatal" };
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

      return new Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>>(list,group);
    }

    public async Task Resolved(int id)
    {
      var item = await this.FindAsync(id);
      item.Resolved = true;
      this.Update(item);
    }
  }
}



