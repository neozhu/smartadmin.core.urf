using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Extensions;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace SmartAdmin.WebUI.Controllers
{
  public class CustomersController : Controller
  {
    private readonly ICustomerService customerService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CustomersController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public CustomersController(ICustomerService customerService,
          IUnitOfWork unitOfWork,
          IWebHostEnvironment webHostEnvironment,
          ILogger<CustomersController> logger)
    {
      this.customerService = customerService;
      this.unitOfWork = unitOfWork;
      this._logger = logger;
      this._webHostEnvironment = webHostEnvironment;
    }

    // GET: Customers
    public IActionResult Index() => View();
    //datagrid 数据源
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      try
      {
        var filters = PredicateBuilder.FromFilter<Customer>(filterRules);
        var total = await this.customerService
                             .Query(filters).CountAsync();
        var pagerows = (await this.customerService
                             .Query(filters)
                           .OrderBy(n => n.OrderBy(sort, order))
                           .Skip(page - 1).Take(rows).SelectAsync())
                           .ToList();
        var pagelist = new { total = total, rows = pagerows };
        return Json(pagelist);
      }
      catch (Exception e)
      {
        throw e;
      }

    }
    //编辑 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> Edit(Customer customer)
    {
      if (ModelState.IsValid)
      {
        try
        {
          this.customerService.Update(customer);
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
    //新建
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<JsonResult> Create([Bind("Name,Contect,PhoneNumber,Address")] Customer customer)
    {
      if (ModelState.IsValid)
      {
        try
        {
          this.customerService.Insert(customer);
          await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true });
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
    //GET: Customers/Delete/:id
    [HttpGet]
    public async Task<JsonResult> Delete(int id)
    {
      try
      {
        await this.customerService.DeleteAsync(id);
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
          await this.customerService.DeleteAsync(key);
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
    public async Task<JsonResult> AcceptChanges(Customer[] customers)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in customers)
          {

            this.customerService.ApplyChanges(item);
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
    public async Task<ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "customers_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var filters = PredicateBuilder.FromFilter<Customer>(filterRules);
      var stream = await this.customerService.ExportExcelAsync(filters, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);
    }
    //上传导入Excel
    [HttpPost]
    public async Task<JsonResult> ImportExcel(List<IFormFile> uploadfiles) {
      try
      {
        var total = 0m;
        var watch = new Stopwatch();
        watch.Start();
        foreach (var formFile in uploadfiles)
        {
          if (formFile.Length > 0)
          {
            var ext = System.IO.Path.GetExtension(formFile.FileName);
            var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var datatable = await NPOIHelper.GetDataTableFromExcelAsync(stream, ext);
            await this.customerService.ImportDataTableAsync(datatable, this.User.Identity.Name);
            await this.unitOfWork.SaveChangesAsync();
            total = total + datatable.Rows.Count;
          }
        }

        watch.Stop();
        //获取当前实例测量得出的总运行时间（以毫秒为单位）
        var elapsedTime = watch.ElapsedMilliseconds.ToString();
        return Json(new { success = true, total, elapsedTime });
      }
      catch (Exception e) {
        return Json(new { success = false, err=e.GetBaseException().Message });
      }
    }

  }
}
