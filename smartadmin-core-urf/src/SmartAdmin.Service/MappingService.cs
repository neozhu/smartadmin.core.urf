using System.Threading;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using URF.Core.EF.Trackable;
using System.ComponentModel;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;
using URF.Core.EF;
using System.Data.Common;
using System.Linq.Dynamic.Core;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SmartAdmin.Dto;

namespace SmartAdmin.Service
{
  public class DataTableImportMappingService : Service<DataTableImportMapping>, IDataTableImportMappingService
  {

    public DataTableImportMappingService(ITrackableRepository<DataTableImportMapping> repository)
       : base(repository) {

    }




    public async Task<IEnumerable<EntityInfo>> GetAssemblyEntitiesAsync()
      => await Task.Run(() =>
                        {
                          var list = new List<EntityInfo>();
                          var asm = Assembly.Load("SmartAdmin.Entity");
                          list = asm.GetTypes()
                                 .Where(type => typeof(Entity).IsAssignableFrom(type))
                                 .SelectMany(type => type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                                 .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                                 .Select(x => new EntityInfo { EntitySetName = x.DeclaringType.Name, FieldName = x.Name, FieldTypeName = x.PropertyType.Name, IsRequired = x.GetCustomAttributes().Where(f => f.TypeId.ToString().IndexOf("Required") >= 0).Any() })
                                 .OrderBy(x => x.EntitySetName).ThenBy(x => x.FieldName).ToList();

                          return list;
                        });


    public async Task GenerateByEnityNameAsync(string entityName)
    {

      var asm = Assembly.Load("SmartAdmin.Entity");
      var list = asm.GetTypes()
             .Where(type => typeof(Entity).IsAssignableFrom(type))
             .SelectMany(type => type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
             .Where(m => m.DeclaringType.Name == entityName &&
                         m.PropertyType.BaseType != typeof(Entity) &&
                       !m.GetCustomAttributes(
                           typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                           true).Any()
                   )
             .Select(x =>
                      new EntityInfo
                      {
                        EntitySetName = x.DeclaringType.Name,
                        FieldName = x.Name,
                        FieldTypeName = x.PropertyType.Name,
                        DefaultValue = (x.GetCustomAttribute(typeof(DefaultValueAttribute)) != null ? (x.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute).Value?.ToString() : null),
                        IsRequired = x.GetCustomAttributes()
                                      .Where(f =>
                                              f.TypeId.ToString().IndexOf("RequiredAttribute") >= 0
                                            ).Any() ||
                                            ((x.PropertyType == typeof(int) && !x.GetCustomAttributes().Where(k => k.TypeId.ToString().IndexOf("KeyAttribute") > 0).Any()) ||
                                             x.PropertyType == typeof(DateTime) ||
                                             x.PropertyType == typeof(decimal)
                                             ),
                        DisplayName = x.GetCustomAttributes()
                                      .Where(f =>
                                              f.TypeId.ToString().IndexOf("DisplayAttribute") >= 0
                                            )
                                      .Select(x => (DisplayAttribute)x)
                                      .FirstOrDefault().Name,
                        IgnoredColumn = x.GetCustomAttributes().Where(k => k.TypeId.ToString().IndexOf("KeyAttribute") > 0).Any()

                      })
             .OrderBy(x => x.EntitySetName)
             .Where(x => x.FieldTypeName != "ICollection`1").ToList();

      var entityType = asm.GetTypes()
             .Where(type => typeof(Entity).IsAssignableFrom(type)).Where(x => x.Name == entityName).First();

      //this.Queryable().Where(x => x.EntitySetName == entityName).Delete();
      foreach (var item in list)
      {
        var any = await this.Queryable().Where(x => x.EntitySetName == entityName && x.FieldName == item.FieldName).AnyAsync();
        if (!any)
        {
          var row = new DataTableImportMapping
          {
            EntitySetName = item.EntitySetName,
            FieldName = item.FieldName,
            IsRequired = item.IsRequired,
            TypeName = item.FieldTypeName,
            DefaultValue = item.DefaultValue,
            IsEnabled = item.IsRequired,
            IgnoredColumn = item.IgnoredColumn,
            SourceFieldName = item.DisplayName
          };
          this.Insert(row);
        }

      }
    }


    public async Task<DataTableImportMapping> FindMappingAsync(string entitySetName, string sourceFieldName) => await this.Queryable().Where(x => x.EntitySetName == entitySetName && x.SourceFieldName == sourceFieldName).FirstOrDefaultAsync();

    public async Task CreateExcelTemplateAsync(string entityname, string filename)
    {
      var mapping = await this.Queryable().Where(x => x.EntitySetName == entityname && x.IsEnabled == true).ToListAsync();
      var finame = new FileInfo(filename);
      if (File.Exists(filename))
      {
        File.Delete(filename);
      }
      var workbook = new XSSFWorkbook();
      var sheet = workbook.CreateSheet(entityname);
      var headerRow = sheet.CreateRow(0);
      //Below loop is create header
      var headstyle = workbook.CreateCellStyle();
      var font = workbook.CreateFont();
      font.FontHeightInPoints = 11;
      font.IsBold = true;
      headstyle.SetFont(font);
      headstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
      headstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
      headstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
      headstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
      headstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
      headstyle.FillPattern = FillPattern.SolidForeground;

      var col = 0;
      foreach (var row in mapping)
      {
        var cell = headerRow.CreateCell(col++);
        cell.SetCellValue(row.SourceFieldName);
        cell.CellStyle = headstyle;
        if (row.TypeName == "DateTime")
        {
          var format = workbook.CreateDataFormat();
          headstyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm");
        }
        else if (row.TypeName.ToLower() == "decimal")
        {
          var format = workbook.CreateDataFormat();
          headstyle.DataFormat = format.GetFormat("#,##0.00");
        }
        else if (row.TypeName.ToLower() == "int")
        {
          var format = workbook.CreateDataFormat();
          headstyle.DataFormat = format.GetFormat("#,##0");
        }
      }

      using (var file = new FileStream(filename, FileMode.Create))
      {
        workbook.Write(file);
      }

    }

  
  

    public async Task ImportDataTableAsync(DataTable datatable)
    {
      foreach (DataRow row in datatable.Rows)
      {
        var entityName = row["实体名称"].ToString();
        var fieldName = row["字段名"].ToString();
        var any = await this.Queryable().Where(x => x.EntitySetName == entityName && x.FieldName == fieldName).AnyAsync();
        if (!any)
        {
          var item = new DataTableImportMapping()
          {
            EntitySetName = row["实体名称"].ToString(),
            DefaultValue = row["默认值"].ToString(),
            FieldName = row["字段名"].ToString(),
            IgnoredColumn = Convert.ToBoolean(row["是否导出"].ToString()),
            IsEnabled = Convert.ToBoolean(row["是否导入"].ToString()),
            IsRequired = Convert.ToBoolean(row["是否必填"].ToString()),
            SourceFieldName = row["Excel列名"].ToString(),
            RegularExpression = row["验证表达式"].ToString(),
            TypeName = row["类型"].ToString(),
          };
          this.Insert(item);
        }
      }

    }
    public async Task Delete(int[] id)
    {
      var items =await this.Queryable().Where(x => id.Contains(x.Id)).ToListAsync();
      foreach (var item in items)
      {
        this.Delete(item);
      }

    }
    public async Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var filters = PredicateBuilder.FromFilter<DataTableImportMapping>(filterRules);
      var expcolopts = await this.Queryable()
                   .Where(x => x.EntitySetName == "DataTableImportMapping")
                   .Select(x => new ExpColumnOpts()
                   {
                     EntitySetName = x.EntitySetName,
                     FieldName = x.FieldName,
                     IgnoredColumn = x.IgnoredColumn,
                     SourceFieldName = x.SourceFieldName
                   }).ToArrayAsync();

      var query = (await this.Query(filters).OrderBy(n => n.OrderBy($"{sort} {order}")).SelectAsync()).ToList();
      var datarows = query.Select(n => new
      {
        EntitySetName = n.EntitySetName,
        FieldName = n.FieldName,
        IsRequired = n.IsRequired,
        TypeName = n.TypeName,
        DefaultValue = n.DefaultValue,
        SourceFieldName = n.SourceFieldName,
        IsEnabled = n.IsEnabled,
        IgnoredColumn = n.IgnoredColumn,
        RegularExpression = n.RegularExpression
      }).ToList();
      return await NPOIHelper.ExportExcelAsync("DataTableImportMapping", datarows, expcolopts);
    }
  }
}
