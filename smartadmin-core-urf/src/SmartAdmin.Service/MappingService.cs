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
            Exportable = item.IgnoredColumn,
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
      var mapping = await this.Queryable().Where(x => x.EntitySetName == "DataTableImportMapping" && ((x.IsEnabled == true) || (x.IsEnabled == false && !(x.DefaultValue == null || x.DefaultValue.Equals(string.Empty))))).ToListAsync();
      if (mapping == null || mapping.Count == 0)
      {
        throw new KeyNotFoundException("没有找到DataTableImportMapping对象的Excel导入配置信息，请执行[系统管理/Excel导入配置]");
      }
      foreach (DataRow row in datatable.Rows)
      {
        var item = new DataTableImportMapping();
        var requiredfield = mapping.Where(x => x.IsRequired == true).FirstOrDefault()?.SourceFieldName;
        if (requiredfield != null && !row.IsNull(requiredfield) && row[requiredfield] != DBNull.Value && Convert.ToString(row[requiredfield]).Trim() != string.Empty)
        {
          foreach (var field in mapping)
          {
            var defval = field.DefaultValue;
            var contain = datatable.Columns.Contains(field.SourceFieldName ?? "");
            if (contain && !row.IsNull(field.SourceFieldName) && row[field.SourceFieldName] != DBNull.Value && row[field.SourceFieldName].ToString() != string.Empty)
            {
              var partnertype = item.GetType();
              var propertyInfo = partnertype.GetProperty(field.FieldName);
              var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
              var safeValue = (row[field.SourceFieldName] == null) ? null : Convert.ChangeType(row[field.SourceFieldName], safetype);
              propertyInfo.SetValue(item, safeValue, null);
            }
            else if (!string.IsNullOrEmpty(defval))
            {
              var codeitemtype = item.GetType();
              var propertyInfo = codeitemtype.GetProperty(field.FieldName);
              if (defval.ToLower() == "now" && propertyInfo.PropertyType == typeof(DateTime))
              {
                propertyInfo.SetValue(item, Convert.ChangeType(DateTime.Now, propertyInfo.PropertyType), null);
              }
              else
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(defval, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
            }
          }
          var has = await this.Queryable().Where(x => x.EntitySetName == item.EntitySetName && x.FieldName == item.FieldName).AnyAsync();
          if (has)
          {
            //this.Queryable().Where(x => x.CodeType == item.CodeType && x.Code == item.Code).UpdateFromQuery(u => new CodeItem()
            //{
            //    Text = item.Text
            //});
          }
          else
          {
            this.Insert(item);
          }
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
    public async Task<Stream> ExportExcelAsync(Expression<Func<DataTableImportMapping, bool>> filters, string sort = "Id", string order = "asc")
    {
      var expcolopts = await this.Queryable()
            .Where(x => x.EntitySetName == "DataTableImportMapping" && x.Exportable)
            .Select(x => new ExpColumnOpts()
            {
              EntitySetName = x.EntitySetName,
              FieldName = x.FieldName,
              IsExportable = x.Exportable,
              SourceFieldName = x.SourceFieldName,
              LineNo = x.LineNo
            }).ToArrayAsync();
      var data = await this.Query(filters).OrderBy(n => n.OrderBy($"{sort} {order}")).SelectAsync();


      return await NPOIHelper.ExportExcelAsync("DataTableImportMapping", data, expcolopts);
    }
  }
}
