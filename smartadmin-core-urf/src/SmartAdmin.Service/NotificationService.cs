using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Data.Models;
using SmartAdmin.Dto;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services;

namespace SmartAdmin.Service
{
  /// <summary>
  /// File: NotificationService.cs
  /// Purpose: Within the service layer, you define and implement 
  /// the service interface and the data contracts (or message types).
  /// One of the more important concepts to keep in mind is that a service
  /// should never expose details of the internal processes or 
  /// the business entities used within the application. 
  /// Created Date: 9/16/2019 10:51:42 AM
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  public class NotificationService : Service<Notification>, INotificationService
  {

    private readonly IDataTableImportMappingService mappingservice;
    public NotificationService(
      ITrackableRepository<Notification> repository,
      IDataTableImportMappingService mappingservice)
        : base(repository)
    {

      this.mappingservice = mappingservice;
    }
    public void Subscribe(SubscribeEventData data) {
      var item = new Notification()
      {
        Content = data.content,
        From = data.from,
        Title = data.title,
        Group = data.group,
        Link = data.url,
        PublishDate = DateTime.Now,
        Publisher=data.publisher,
        To = data.to

      };
      this.Insert(item);

      }



    public async Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var filters = PredicateBuilder.FromFilter<Notification>(filterRules);
      var expcolopts = await this.mappingservice.Queryable()
         .Where(x => x.EntitySetName == "Notification" && x.Exportable)
         .Select(x => new ExpColumnOpts()
         {
           EntitySetName = x.EntitySetName,
           FieldName = x.FieldName,
           IsExportable = x.Exportable,
           SourceFieldName = x.SourceFieldName,
           LineNo = x.LineNo
         })
         .ToArrayAsync();
      var notifications = await this.Query(filters).OrderBy(n => n.OrderBy($"{sort} {order}")).SelectAsync();

      return await NPOIHelper.ExportExcelAsync("Notification", notifications, expcolopts);
    }
    public async Task Delete(int[] id)
    {
      var items = await this.Queryable().Where(x => id.Contains(x.Id)).ToListAsync();
      foreach (var item in items)
      {
        this.Delete(item);
      }

    }
  }
}



