using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SmartAdmin.Data.Models;
using SmartAdmin.Service;
using URF.Core.Abstractions;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ICompanyService  _companyService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        ICompanyService companyService,
        IUnitOfWork unitOfWork,
        ILogger<HomeController> logger)
    {
      _companyService = companyService;
      _unitOfWork = unitOfWork;
      this.logger = logger;
      this.logger.LogInformation("访问首页");
    }

    public IActionResult Index() => View();


  }
}
