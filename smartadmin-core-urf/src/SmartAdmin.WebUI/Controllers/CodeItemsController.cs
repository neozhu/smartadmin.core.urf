using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Extensions;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace SmartAdmin.WebUI.Controllers
{
  public class CodeItemsController : Controller
  {

    private readonly ICodeItemService _codeItemService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<CodeItemsController> _logger;
    public CodeItemsController(ICodeItemService codeItemService,
      IWebHostEnvironment webHostEnvironment,
      ILogger<CodeItemsController> logger,
          IUnitOfWork unitOfWork)
    {
      _codeItemService = codeItemService;
      _unitOfWork = unitOfWork;
      _webHostEnvironment = webHostEnvironment;
   _logger = logger;
    }

    // GET: CodeItems/Index
    public IActionResult Index() => View();


    // Get :CodeItems/PageList
    // For Index View Boostrap-Table load  data 
    [HttpGet]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<CodeItem>(filterRules);
      var total = await this._codeItemService
                         .Query(filters).CountAsync();
      var pagerows = (await _codeItemService
                    .Query(filters)
                    .OrderBy(n => n.OrderBy(sort, order))
                           .Skip(page - 1).Take(rows)
                           .SelectAsync())
                      .Select(n => new
                      {
                        Multiple = n.Multiple,
                        CodeType = n.CodeType,
                        Id = n.Id,
                        Code = n.Code,
                        Text = n.Text,
                        Description = n.Description,
                        IsDisabled = n.IsDisabled
                      })
                      .ToList();
      var pagelist = new { total = total, rows = pagerows };
      return Json(pagelist);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateJavascript()
    {
      var jsfile = Path.Combine(this._webHostEnvironment.WebRootPath, "js", "jquery.extend.formatter.js");
      await this._codeItemService.UpdateJavascriptAsync(jsfile);
      return Json(new { success = true });
    }

    public async Task<IActionResult> DeleteChecked(int[] id)
    {
      foreach (var key in id)
      {
        await this._codeItemService.DeleteAsync(key);
      }
      await this._unitOfWork.SaveChangesAsync();
      return Json(new { success = true } );
    }

    [HttpPost]
    public async Task<ActionResult> SaveData(CodeItem[] codeitems)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in codeitems)
          {
            this._codeItemService.ApplyChanges(item);
          }
          var result = await this._unitOfWork.SaveChangesAsync();
          return Json(new { success = true, result });
        }

        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
      }
    }
    //导出Excel
    [HttpPost]
    public async Task<ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "codeitems_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await _codeItemService.ExportExcelAsync(filterRules, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);

    }
    //导入excel
    [HttpPost]
    public async Task<IActionResult> ImportExcel()
    {
      try
      {
        var watch = new Stopwatch();
        watch.Start();
        var total = 0;
        if (Request.Form.Files.Count > 0)
        {
          for (var i = 0; i < this.Request.Form.Files.Count; i++)
          {
            var model = Request.Form["model"].FirstOrDefault() ?? "codeitems";
            var folder = Request.Form["folder"].FirstOrDefault() ?? "codeitems";
            var autosave = Convert.ToBoolean(Request.Form["autosave"].FirstOrDefault());
            var properties = (Request.Form["properties"].FirstOrDefault()?.Split(','));
            var file = Request.Form.Files[i];
            var filename = file.FileName;
            var contenttype = file.ContentType;
            var size = file.Length;
            var ext = Path.GetExtension(filename);
            var path = Path.Combine(this._webHostEnvironment.ContentRootPath, "UploadFiles", folder);
            if (!Directory.Exists(path))
            {
              Directory.CreateDirectory(path);
            }
            var datatable = await NPOIHelper.GetDataTableFromExcelAsync(file.OpenReadStream(), ext);
            await this._codeItemService.ImportDataTableAsync(datatable);
            await this._unitOfWork.SaveChangesAsync();
            total = datatable.Rows.Count;
            if (autosave)
            {
              var filepath = Path.Combine(path, filename);
              file.OpenReadStream().Position = 0;

              using (var stream = System.IO.File.Create(filepath))
              {
                await file.CopyToAsync(stream);
              }
            }

          }
        }
        watch.Stop();
        return Json(new { success = true, total = total, elapsedTime = watch.ElapsedMilliseconds });
      }
      catch (Exception e)
      {
        this._logger.LogError(e, "Excel导入失败");
        return this.Json(new { success = false, err = e.GetBaseException().Message });
      }
    }
    //下载模板
    public async Task<IActionResult> Download(string file)
    {
 
      this.Response.Cookies.Append("fileDownload", "true");
      var path = Path.Combine(this._webHostEnvironment.ContentRootPath, file);
      var downloadFile = new FileInfo(path);
      if (downloadFile.Exists)
      {
        var fileName = downloadFile.Name;
        var mimeType = MimeTypeConvert.FromExtension(downloadFile.Extension);
        var fileContent = new byte[Convert.ToInt32(downloadFile.Length)];
        using (var fs = downloadFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          await fs.ReadAsync(fileContent, 0, Convert.ToInt32(downloadFile.Length));
        }
        return this.File(fileContent, mimeType, fileName);
      }
      else
      {
        throw new FileNotFoundException($"文件 {file} 不存在!");
      }
    }
  }
}
