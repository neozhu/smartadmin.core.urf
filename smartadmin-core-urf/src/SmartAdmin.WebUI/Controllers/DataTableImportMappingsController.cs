using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using URF.Core.Abstractions;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class DataTableImportMappingsController : Controller
  {
    private readonly IDataTableImportMappingService _dataTableImportMappingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompaniesController> logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DataTableImportMappingsController(IDataTableImportMappingService dataTableImportMappingService,
      IWebHostEnvironment webHostEnvironment,
      ILogger<CompaniesController> logger,
      IUnitOfWork unitOfWork)
    {
      _webHostEnvironment = webHostEnvironment;
      _dataTableImportMappingService = dataTableImportMappingService;
      _unitOfWork = unitOfWork;
      this.logger = logger;
    }

    // GET: DataTableImportMappings/Index
    public ActionResult Index()
    {

      //var datatableimportmappings  = _dataTableImportMappingService.Queryable().AsQueryable();
      //return View(datatableimportmappings  );
      return View();
    }

    // Get :DataTableImportMappings/PageList
    // For Index View Boostrap-Table load  data 
    [HttpGet]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "desc", string filterRules = "")
    {
      try
      {
        var filters = PredicateBuilder.FromFilter<DataTableImportMapping>(filterRules);
        var total = await this._dataTableImportMappingService
                           .Query(filters).CountAsync();
      var pagerows = (await this._dataTableImportMappingService
                             .Query(filters)
                           .OrderBy(n => n.OrderBy($"{sort} {order}").ThenBy(x=>x.LineNo))
                           .Skip(page - 1).Take(rows).SelectAsync())
                           .Select(n => new
             {
          Id = n.Id,
          EntitySetName = n.EntitySetName,
          DefaultValue = n.DefaultValue,
          FieldName = n.FieldName,
          IsRequired = n.IsRequired,
          TypeName = n.TypeName,
          SourceFieldName = n.SourceFieldName,
          IsEnabled = n.IsEnabled,
                             Exportable = n.Exportable,
          RegularExpression = n.RegularExpression,
          LineNo=n.LineNo
        }).ToList();
        var pagelist = new { total = total, rows = pagerows };
        return Json(pagelist);
      }
      catch (Exception e) {
        throw e;
        }
    }

    [HttpPost]
    public async Task<ActionResult> SaveData(DataTableImportMapping[] datatableimportmappings)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in datatableimportmappings)
          {
            this._dataTableImportMappingService.ApplyChanges(item);
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


    [HttpGet]
    public async Task<ActionResult> GetAllEntites()
    {
      var list =await _dataTableImportMappingService.GetAssemblyEntitiesAsync();
      var rows = list.Select(x => new { Name = x.EntitySetName, Value = x.EntitySetName }).Distinct();
      return Json(rows);

    }
    [HttpPost]
    public async Task<ActionResult> Generate(string entityname)
    {
      await _dataTableImportMappingService.GenerateByEnityNameAsync(entityname);
      await _unitOfWork.SaveChangesAsync();
      return Json(new { success = true });
    }
    public async Task<ActionResult> CreateExcelTemplate(string entityname)
    {

      var count = await this._dataTableImportMappingService.Queryable().Where(x => x.EntitySetName == entityname && x.IsEnabled == true).AnyAsync();
      if (count)
      {
        var filename = Path.Combine(this._webHostEnvironment.ContentRootPath,"ExcelTemplate" ,entityname + ".xlsx");
       await  _dataTableImportMappingService.CreateExcelTemplateAsync(entityname, filename);

        return Json(new { success = true });
      }
      else
      {
        return Json(new { success = false, message = "没有找到[" + entityname + "]配置信息请,先执行【生成】mapping关系" });
      }
    }

    [HttpPost]
    public async Task<ActionResult> ImportConfig()
    {
      if (Request.Form.Files.Count > 0)
      {
        for (var i = 0; i < this.Request.Form.Files.Count; i++)
        {
          var label = Request.Form["label"];
          var file = Request.Form.Files[i];
          if (file != null && file.Length > 0)
          {
            var filename = file.FileName;
            var contenttype = file.ContentType;
            var size = file.Length;
            var ext = Path.GetExtension(filename);

            var folder = Path.Combine(this._webHostEnvironment.ContentRootPath, "UploadFiles"); 
            if (!Directory.Exists(folder))
            {
              Directory.CreateDirectory(folder);
            }
            var virtualPath = Path.Combine(folder, filename);
            // 文件系统不能使用虚拟路径
            //string path = this.Server.MapPath(virtualPath);
           
            var datatable = await NPOIHelper.GetDataTableFromExcelAsync(file.OpenReadStream(), ext);
            await this._dataTableImportMappingService.ImportDataTableAsync(datatable);
            await this._unitOfWork.SaveChangesAsync();
            file.OpenReadStream().Position = 0;
            using (var stream = System.IO.File.Create(virtualPath))
            {
              await file.CopyToAsync(stream);
            }

            return Content(filename);
          }
        }
        
      }

      return Content(null);
    }
    //删除选中的记录
    [HttpPost]
    public async Task<JsonResult> DeleteChecked(int[] id)
    {
      if (id == null)
      {
        throw new ArgumentNullException(nameof(id));
      }
      try
      {
        await this._dataTableImportMappingService.Delete(id);
        await this._unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }
    }

    [HttpDelete]
    public JsonResult Revert()
    {
      var req = Request.Body;
      var filename = new StreamReader(req).ReadToEnd();
      if (!string.IsNullOrEmpty(filename))
      {
     
        var path = Path.Combine(this._webHostEnvironment.ContentRootPath, "UploadFiles", filename);
        if (System.IO.File.Exists(path))
        {
          System.IO.File.Delete(path);
        }
      }
      return this.Json(new { success = true });
    }

    [HttpPost]
    public async Task<ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var filters = PredicateBuilder.FromFilter<DataTableImportMapping>(filterRules);
      var fileName = "excelconfiguration_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await this._dataTableImportMappingService.ExportExcelAsync(filters, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);
    }



  }
}
