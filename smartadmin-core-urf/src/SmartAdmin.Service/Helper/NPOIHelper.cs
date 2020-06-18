using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SmartAdmin.Data.Models;

namespace SmartAdmin
{
  public static class NpoiExtensions
  {
    public static string GetFormattedCellValue(this ICell cell, IFormulaEvaluator eval = null)
    {
      if (cell != null)
      {
        switch (cell.CellType)
        {
          case CellType.String:
            return cell.StringCellValue;

          case CellType.Numeric:
            if (DateUtil.IsCellDateFormatted(cell))
            {
              try
              {
                var date = cell.DateCellValue;

                return date.ToString("yyyy-MM-dd HH:mm:ss");
              }
              catch (NullReferenceException)
              {
                return DateTime.FromOADate(cell.NumericCellValue).ToString();
              }
            }
            else
            {
              return cell.NumericCellValue.ToString();
            }

          case CellType.Boolean:
            return cell.BooleanCellValue ? "TRUE" : "FALSE";

          case CellType.Formula:
            if (eval != null)
            {
              return GetFormattedCellValue(eval.EvaluateInCell(cell));
            }
            else
            {
              return cell.CellFormula;
            }

          case CellType.Error:
            return FormulaError.ForInt(cell.ErrorCellValue).String;
        }
      }
      // null or blank cell, or unknown cell type
      return null;
    }
  }

  public class NPOIHelper
  {
    public static Task<DataTable> GetDataTableFromExcelAsync(Stream stream, string type = ".xls") => Task.Run(() =>
    {
      IWorkbook workbook = null;
      IFormulaEvaluator eval = null;
      if (type == ".xlsx") // 2007版本
      {
        workbook = new XSSFWorkbook(stream);
        eval = new XSSFFormulaEvaluator(workbook);
      }
      else  // 2003版本
      {
        workbook = new HSSFWorkbook(stream);
        eval = new HSSFFormulaEvaluator(workbook);
      }

      var sheet = workbook.GetSheetAt(0); // zero-based index of your target sheet

      var dt = new DataTable(sheet.SheetName);

      // write header row
      var headerRow = sheet.GetRow(0);
      foreach (ICell headerCell in headerRow)
      {
        dt.Columns.Add(headerCell.ToString().Trim());
      }

      // write the rest
      var rowIndex = 0;
      foreach (IRow row in sheet)
      {
        // skip header row
        if (rowIndex++ == 0)
        {
          continue;
        }

        var dataRow = dt.NewRow();
        var array = new string[dt.Columns.Count];
        for (var i = 0; i < dt.Columns.Count; i++)
        {
          var cell = row.GetCell(i);
          var val = cell.GetFormattedCellValue(eval);
          array[i] = val;
        }
        dataRow.ItemArray = array;
        dt.Rows.Add(dataRow);
      }

      return dt;

    });

    public static Task<MemoryStream> ExportExcelAsync<T>(string name, IEnumerable<T> list, ExpColumnOpts[] colopts) => Task.Run(() =>
    {
      //var ignoredColumns = colopts.Where(x => x.IgnoredColumn == true);
      //var columns= colopts.Where(x => x.IgnoredColumn == false);
      var stream = new MemoryStream();
      var workbook = new XSSFWorkbook();
      var PropertyInfos = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
      var sheet = workbook.CreateSheet(name);
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
      for (var i = 0; i < PropertyInfos.Length; i++)
      {
       

        var fieldname = PropertyInfos[i].Name;
        var fieldtype = PropertyInfos[i].PropertyType;
        if (colopts != null &&
       (colopts.Where(n => n.FieldName == fieldname && n.IgnoredColumn == false).Any() ||
       !colopts.Any(x => x.FieldName == fieldname)
       )
       )
        {
          continue;
        }
        var displayname = colopts.Where(x => x.FieldName == fieldname).FirstOrDefault()?.SourceFieldName;
        var cell = headerRow.CreateCell(col++);
        cell.SetCellValue(displayname ?? fieldname);
        cell.CellStyle = headstyle;

      }
      var index = 1;
      //for (var i = 0; i < list.Count(); i++)
      foreach(var item in list)
      {
        var row = sheet.CreateRow(index++);
        //var item = list;
        col = 0;
        var style = workbook.CreateCellStyle();
        style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
        style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        for (var l = 0; l < PropertyInfos.Length; l++)
        {
          var fieldname = PropertyInfos[l].Name;
          var fieldtype = PropertyInfos[l].PropertyType;
          if (colopts != null &&
       (colopts.Where(n => n.FieldName == fieldname && n.IgnoredColumn == false).Any() ||
       !colopts.Any(x => x.FieldName == fieldname)
       )
       )
          {
            continue;
          }

          if (fieldtype == typeof(decimal) || fieldtype == typeof(Nullable<decimal>))
          {
            var format = workbook.CreateDataFormat();
            style.DataFormat = format.GetFormat("#,##0.00");
          }
          else if (fieldtype == typeof(int) || fieldtype == typeof(Nullable<int>))
          {
            var format = workbook.CreateDataFormat();
            style.DataFormat = format.GetFormat("#,##0");
          }
          else if (fieldtype == typeof(DateTime) || fieldtype == typeof(Nullable<DateTime>))
          {
            if (fieldname.IndexOf("time", StringComparison.OrdinalIgnoreCase) > -1)
            {
              var format = workbook.CreateDataFormat();
              style.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm");
            }
            else
            {
              var format = workbook.CreateDataFormat();
              style.DataFormat = format.GetFormat("yyyy-MM-dd");
            }
          }

          var cell = row.CreateCell(col++);
          var val = item.GetType().GetProperty(fieldname).GetValue(item, null);
          if (fieldtype == typeof(decimal) || fieldtype == typeof(Nullable<decimal>))
          {
            if (val != null)
            {
              cell.SetCellValue(Convert.ToDouble(val));
            }
          }
          else if (fieldtype == typeof(int) || fieldtype == typeof(Nullable<int>))
          {
            if (val != null)
            {
              cell.SetCellValue(Convert.ToInt32(val));
            }
          }
          else if (fieldtype == typeof(DateTime) || fieldtype == typeof(Nullable<DateTime>))
          {
            if (val != null)
            {
              cell.SetCellValue(Convert.ToDateTime(val));
            }
          }
          else
          {
            cell.SetCellValue(val?.ToString());
          }
          cell.CellStyle = style;
          sheet.AutoSizeColumn(col);
        }
      }
      var bookstream = new MemoryStream();
      workbook.Write(bookstream);
      var byteArray = bookstream.ToArray();
      stream.Write(byteArray, 0, byteArray.Length);
      stream.Seek(0, SeekOrigin.Begin);
      return stream;
    });
  }


}
