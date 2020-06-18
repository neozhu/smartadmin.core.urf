using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartAdmin.Service;
using URF.Core.Abstractions;

namespace SmartAdmin.WebUI.Controllers
{
  public class FileUploadController : Controller
  {
    private readonly ICompanyService _companyService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FileUploadController> logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FileUploadController(
        ICompanyService companyService,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment,
        ILogger<FileUploadController> logger)
    {
      _companyService = companyService;
      _unitOfWork = unitOfWork;
      _webHostEnvironment = webHostEnvironment;
      this.logger = logger;
    }
    public IActionResult Index()
    {
      return View();
    }

    //删除文件
    [HttpPost]
    public JsonResult Remove(string filename = "")
    {
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
    private string GetMimeType(string str)
    {
      var ContentTypeStr = "application/octet-stream";
      var fileExtension = str.ToLower();
      switch (fileExtension)
      {
        case ".mp3":
          ContentTypeStr = "audio/mpeg3";
          break;
        case ".mpeg":
          ContentTypeStr = "video/mpeg";
          break;
        case ".jpg":
          ContentTypeStr = "image/jpeg";
          break;
        case ".bmp":
          ContentTypeStr = "image/bmp";
          break;
        case ".gif":
          ContentTypeStr = "image/gif";
          break;
        case ".doc":
          ContentTypeStr = "application/msword";
          break;
        case ".css":
          ContentTypeStr = "text/css";
          break;
        case ".html":
          ContentTypeStr = "text/html";
          break;
        case ".htm":
          ContentTypeStr = "text/html";
          break;
        case ".swf":
          ContentTypeStr = "application/x-shockwave-flash";
          break;
        case ".exe":
          ContentTypeStr = "application/octet-stream";
          break;
        case ".inf":
          ContentTypeStr = "application/x-texinfo";
          break;
        case ".xls":
        case ".xlsx":
          ContentTypeStr = "application/vnd.ms-excel";
          break;
        default:
          ContentTypeStr = "application/octet-stream";
          break;
      }
      return ContentTypeStr;
    }
  }
}
