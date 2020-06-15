using System;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Services;
using System.Collections.Generic;
namespace SmartAdmin.Service
{
  /// <summary>
  /// File: ILogService.cs
  /// Purpose: Service interfaces. Services expose a service interface
  /// to which all inbound messages are sent. You can think of a service interface
  /// as a façade that exposes the business logic implemented in the application
  /// Created Date: 9/19/2019 8:51:50 AM
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  public interface ILogService : IService<Log>
  {

    Task Resolved(int id);
    Task<Tuple<IEnumerable<dynamic>, IEnumerable<dynamic>>>  GetSummaryData();
  }
}
