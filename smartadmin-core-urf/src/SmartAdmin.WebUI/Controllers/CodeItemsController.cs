using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace SmartAdmin.WebUI.Controllers
{
  public class CodeItemsController : Controller
  {

    private readonly ICodeItemService _codeItemService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public CodeItemsController(ICodeItemService codeItemService,
      IWebHostEnvironment webHostEnvironment,
          IUnitOfWork unitOfWork)
    {
      _codeItemService = codeItemService;
      _unitOfWork = unitOfWork;
      _webHostEnvironment = webHostEnvironment;
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
    public async Task<ActionResult> UpdateJavascript()
    {
      var jsfile = Path.Combine(this._webHostEnvironment.WebRootPath, "js", "jquery.extend.formatter.js");
      await this._codeItemService.UpdateJavascriptAsync(jsfile);
      return Json(new { success = true });
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
  }
}
