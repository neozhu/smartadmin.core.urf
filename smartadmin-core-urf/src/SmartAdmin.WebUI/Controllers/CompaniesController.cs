using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Extensions;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace SmartAdmin.WebUI.Controllers
{
  public class CompaniesController : Controller
  {
    private  readonly ICompanyService companyService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CompaniesController> logger;

    public CompaniesController(ICompanyService companyService,
          IUnitOfWork unitOfWork,
          ILogger<CompaniesController> logger)
    {
      this.companyService = companyService;
      this.unitOfWork = unitOfWork;
      this.logger = logger;
    }

    // GET: Companies
    public IActionResult Index()=> View();

    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      try
      {
        var filters = PredicateBuilder.FromFilter<Company>(filterRules);
        var total = await this.companyService
                             .Query(filters).CountAsync();
        var pagerows = (await this.companyService
                             .Query(filters)
                           .OrderBy(n => n.OrderBy(sort, order))
                           .Skip(page - 1).Take(rows).SelectAsync())
                           .Select(n => new
                           {
                             Id = n.Id,
                             Name = n.Name,
                             Code = n.Code,
                             Address = n.Address,
                             Contect = n.Contect,
                             PhoneNumber = n.PhoneNumber,
                             RegisterDate = n.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss")
                           }).ToList();
        var pagelist = new { total = total, rows = pagerows };
        return Json(pagelist);
      }
      catch(Exception e) {
        throw e;
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> Edit(Company company)
    {
      if (ModelState.IsValid)
      {
        try
        {
          this.companyService.Update(company);

          var result = await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true, result = result });
        }
         catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
        //DisplayErrorMessage(modelStateErrors);
      }
      //return View(work);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
   
    public async Task<JsonResult> Create([Bind("Name,Code,Address,Contect,PhoneNumber,RegisterDate")] Company company)
    {
      if (ModelState.IsValid)
      {
        try
        {
          this.companyService.Insert(company);
       await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true});
        }
        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }

        //DisplaySuccessMessage("Has update a Work record");
        //return RedirectToAction("Index");
      }
      else
       {
        var modelStateErrors = string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
        //DisplayErrorMessage(modelStateErrors);
      }
      //return View(work);
    }
    //删除当前记录
    //GET: Companies/Delete/:id
    [HttpGet]
    public async Task<JsonResult> Delete(int id)
    {
      try
      {
        await this.companyService.DeleteAsync(id);
        await this.unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
     
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }
    }
    //删除选中的记录
    [HttpPost]
    public async Task<JsonResult> DeleteChecked(int[] id)
    {
      try
      {
        foreach (var key in id)
        {
          await this.companyService.DeleteAsync(key);
        }
        await this.unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }
    }
    //保存datagrid编辑的数据
    [HttpPost]
    public async Task<JsonResult> AcceptChanges(Company[] companies)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in companies)
          {
            this.companyService.ApplyChanges(item);
          }
          var result = await this.unitOfWork.SaveChangesAsync();
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
    public async Task<IActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "compnay" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await this.companyService.ExportExcelAsync(filterRules, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);
    }
  }
}
