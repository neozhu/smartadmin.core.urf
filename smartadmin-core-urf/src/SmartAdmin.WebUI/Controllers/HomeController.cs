using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SmartAdmin.Entity.Models;
using SmartAdmin.Service;
using URF.Core.Abstractions;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ICompanyService  _companyService;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(
        ICompanyService companyService,
        IUnitOfWork unitOfWork)
    {
      _companyService = companyService;
      _unitOfWork = unitOfWork;
    }

    public IActionResult Index() => View();


  }
}
