using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Data.Models;
using SmartAdmin.Entity.Models;
using SmartAdmin.Service;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;
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
     
  }
}
