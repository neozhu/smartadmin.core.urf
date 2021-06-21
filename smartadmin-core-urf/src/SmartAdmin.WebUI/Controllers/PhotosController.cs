using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartAdmin.Application.Photos.Commands;
using SmartAdmin.Application.Photos.Queries;

namespace SmartAdmin.WebUI.Controllers
{
  public class PhotosController : Controller
  {
    private readonly IMediator mediator;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ILogger<PhotosController> logger;

    public PhotosController(
      IMediator mediator,
          IWebHostEnvironment webHostEnvironment,
          ILogger<PhotosController> logger
      )
    {
      this.mediator = mediator;
      this.webHostEnvironment = webHostEnvironment;
      this.logger = logger;
    }
    public IActionResult Index()
    {
      return View();
    }

    public async Task<JsonResult> GetData(PhotoFliterQuery request) {
      var result = await this.mediator.Send(request);
      return Json(result);

    }
    [HttpPost]
    public async Task<JsonResult> Upload(List<IFormFile> file,string name,string tag) {
      foreach(var fi in file)
      {
        var filename = fi.FileName;
        var stream = new MemoryStream();
        await fi.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);

        var request = new AddPhotoCommand()
        {
          FileName = filename,
          Stream = stream,
          Path = "",
          Size = stream.Length
        };
        var result = this.mediator.Send(request);
      }
      return Json(new { success = true });
    }
  }
}
