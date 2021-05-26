using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Dto;
using TrackableEntities.Common.Core;
using URF.Core.Services;
using System.Linq.Dynamic.Core;
using URF.Core.EF.Trackable;
using SmartAdmin.Service.Common;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartAdmin.Service
{
  public class ServiceX<TEntity> : Service<TEntity>, IServiceX<TEntity> where TEntity : class, ITrackable
    {
        private readonly IRepositoryX<TEntity> repository;
    public readonly IDataTableImportMappingService _mappingservice;
    public readonly ILogger<TEntity> _logger;
    private readonly IExcelService excelService;

    protected ServiceX(
      IDataTableImportMappingService mappingservice,
      ILogger<TEntity> logger,
      IExcelService excelService,
      IRepositoryX<TEntity> repository) : base(repository)
        {
            this.repository = repository;
          _logger = logger;
      this.excelService = excelService;
      _mappingservice = mappingservice;
        }

        public TEntity Find(object[] keyValues, CancellationToken cancellationToken = default)
        {
             return this.repository.Find(keyValues, cancellationToken);
        }
    public async Task ImportData(Stream stream)
    {
      var datatable = await this.excelService.ReadDataTable(stream);
      var entityName = typeof(TEntity).Name;
      var mapping = await this._mappingservice.Queryable()
                        .Where(x => x.EntitySetName == entityName &&
                           (x.IsEnabled == true || (x.IsEnabled == false && x.DefaultValue != null))
                           )
                        .ToListAsync();
      if (mapping.Count == 0)
      {
        throw new NullReferenceException($"没有找到${entityName}对象的Excel导入配置信息，请执行[系统管理/Excel导入配置]");
      }
      foreach (DataRow row in datatable.Rows)
      {

        var requiredfield = mapping.Where(x => x.IsRequired == true && x.IsEnabled == true && x.DefaultValue == null).FirstOrDefault()?.SourceFieldName;
        if (requiredfield != null || !row.IsNull(requiredfield))
        {
          var item = Activator.CreateInstance<TEntity>();
          var entitytype = item.GetType();
          foreach (var field in mapping)
          {
            var defval = field.DefaultValue;
            var contain = datatable.Columns.Contains(field.SourceFieldName ?? "");
            if (contain && !row.IsNull(field.SourceFieldName))
            {
              var propertyInfo = entitytype.GetProperty(field.FieldName);
              var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
              var safeValue = (row[field.SourceFieldName] == null) ? null : Convert.ChangeType(row[field.SourceFieldName], safetype);
              propertyInfo.SetValue(item, safeValue, null);
            }
            else if (!string.IsNullOrEmpty(defval))
            {
               var propertyInfo = entitytype.GetProperty(field.FieldName);
              if (string.Equals(defval, "now", StringComparison.OrdinalIgnoreCase) && (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(Nullable<DateTime>)))
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(DateTime.Now, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
              else if (string.Equals(defval, "guid", StringComparison.OrdinalIgnoreCase))
              {
                propertyInfo.SetValue(item, Guid.NewGuid().ToString(), null);
              }
              //else if (string.Equals(defval, "user", StringComparison.OrdinalIgnoreCase))
              //{
              //  propertyInfo.SetValue(item, username, null);
              //}
              else
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(defval, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
            }
          }
          this.Insert(item);
        }
      }
    }
    public async Task<Stream> Export(System.Linq.Expressions.Expression<Func<TEntity,bool>> filters, string sort = "Id", string order = "asc")
    {
      var entityName = typeof(TEntity).Name;
      var expcolopts = await this._mappingservice.Queryable()
             .Where(x => x.EntitySetName == entityName && x.Exportable)
             .OrderBy(x=>x.LineNo)
             .Select(x => new ExpColumnOpts()
             {
               EntitySetName = x.EntitySetName,
               FieldName = x.FieldName,
               IsExportable = x.Exportable,
               SourceFieldName = x.SourceFieldName,
               LineNo=x.LineNo
             }).ToArrayAsync();
      var mappers = new Dictionary<string, Func<TEntity, object>>();
      foreach (var opt in expcolopts) {
        var func = DynamicExpressionParser.ParseLambda<TEntity, object>(ParsingConfig.Default, false,$"x=>x.{opt.FieldName}", opt).Compile();
        mappers.Add(opt.SourceFieldName, func);
        }
      var result = await this.Query(filters).OrderBy(n => n.OrderBy($"{sort} {order}")).SelectAsync();
      return await this.excelService.Export(result, mappers);
    }

    public Task<TEntity> CreateOrEdit(TEntity entity) {
      var item = entity as Entity;
      if (item.Id <= 0)
      {
        this.Insert(entity);
      }
      else
      {
        this.Update(entity);
      }
      return Task.FromResult(entity);
      }
  }
}
