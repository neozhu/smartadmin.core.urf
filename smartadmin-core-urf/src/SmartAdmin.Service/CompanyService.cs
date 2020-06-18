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

// Sample to extend ProductService, scoped to only ProductService vs. application wide
namespace SmartAdmin.Service
{
  public class CompanyService : Service<Company>, ICompanyService
  {
    private readonly IDataTableImportMappingService mappingservice;
    private readonly ILogger<CompanyService> logger;
    public CompanyService(
      IDataTableImportMappingService mappingservice,
      ILogger<CompanyService> logger,
      ITrackableRepository<Company> repository) : base(repository)
    {
      this.mappingservice = mappingservice;
      this.logger = logger;
    }

    public async Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var filters = PredicateBuilder.FromFilter<Company>(filterRules);
      var expcolopts = await this.mappingservice.Queryable()
             .Where(x => x.EntitySetName == "Company")
             .Select(x => new ExpColumnOpts()
             {
               EntitySetName = x.EntitySetName,
               FieldName = x.FieldName,
               IgnoredColumn = x.IgnoredColumn,
               SourceFieldName = x.SourceFieldName
             }).ToArrayAsync();

      var works = (await this.Query(filters).OrderBy(n => n.OrderBy(sort, order)).SelectAsync()).ToList();
      var datarows = works.Select(n => new
      {
        Id = n.Id,
        Name = n.Name,
        Code = n.Code,
        Address = n.Address,
        Contect = n.Contect,
        PhoneNumber = n.PhoneNumber,
        RegisterDate = n.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss")
      }).ToList();
      return await NPOIHelper.ExportExcelAsync("Company", datarows, expcolopts);
    }

    public async Task ImportDataTableAsync(DataTable datatable)
    {
      var mapping = await this.mappingservice.Queryable()
                        .Where(x => x.EntitySetName == "Company" &&
                           (x.IsEnabled == true || (x.IsEnabled == false && x.DefaultValue != null))
                           ).ToListAsync();
      if (mapping.Count == 0)
      {
        throw new  NullReferenceException("没有找到Work对象的Excel导入配置信息，请执行[系统管理/Excel导入配置]");
      }
      foreach (DataRow row in datatable.Rows)
      {

        var requiredfield = mapping.Where(x => x.IsRequired == true && x.IsEnabled == true && x.DefaultValue == null).FirstOrDefault()?.SourceFieldName;
        if (requiredfield != null || !row.IsNull(requiredfield))
        {
          var item = new Company();
          foreach (var field in mapping)
          {
            var defval = field.DefaultValue;
            var contain = datatable.Columns.Contains(field.SourceFieldName ?? "");
            if (contain && !row.IsNull(field.SourceFieldName))
            {
              var worktype = item.GetType();
              var propertyInfo = worktype.GetProperty(field.FieldName);
              var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
              var safeValue = (row[field.SourceFieldName] == null) ? null : Convert.ChangeType(row[field.SourceFieldName], safetype);
              propertyInfo.SetValue(item, safeValue, null);
            }
            else if (!string.IsNullOrEmpty(defval))
            {
              var worktype = item.GetType();
              var propertyInfo = worktype.GetProperty(field.FieldName);
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
              else if (string.Equals(defval, "user", StringComparison.OrdinalIgnoreCase))
              {
                propertyInfo.SetValue(item, "", null);
              }
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

    // Example, adding synchronous Single method
    public Company Single(Expression<Func<Company, bool>> predicate)
    {
      
      return this.Repository.Queryable().Single(predicate);

    }
  }
}
