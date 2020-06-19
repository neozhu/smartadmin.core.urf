using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SmartAdmin.Data.Models;
using SmartAdmin.Dto;
using SmartAdmin.Service;
using URF.Core.Abstractions;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ICapPublisher _eventBus;
    private readonly ICompanyService  _companyService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HomeController> logger;

    public HomeController(
      ICapPublisher eventBus,
        ICompanyService companyService,
        IUnitOfWork unitOfWork,
        ILogger<HomeController> logger)
    {
      _eventBus = eventBus;
      _companyService = companyService;
      _unitOfWork = unitOfWork;
      this.logger = logger;
      this.logger.LogInformation("访问首页");
      _eventBus.Publish("smartadmin.eventbus", new SubscribeEventData() {
         content="访问首页",
          from= "HomeController",
           group="操作日志",
            title= "访问首页",
             url="/Home/Index"
      });
    }

    public IActionResult Index() => View();


  }
}
