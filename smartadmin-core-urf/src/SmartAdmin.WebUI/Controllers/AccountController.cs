using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Controllers
{

  [Authorize]
  public class AccountController : Controller
  {
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConfiguration _config;
    public AccountController(
      IWebHostEnvironment webHostEnvironment,
      SignInManager<ApplicationUser> signInManager,
      UserManager<ApplicationUser> userManager,
      IConfiguration config,
      ILogger<AccountController> logger
      )
    {
      _webHostEnvironment = webHostEnvironment;
      _logger = logger;
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
    }

    public IActionResult Profile() => View();

    [HttpPost]
    public async Task<JsonResult> UpdateAvatar(string base64str)
    {
      var username = this.User.Identity.Name;
      var identity = await this._userManager.FindByNameAsync(username);
      this.saveToAvatar(base64str, username);
      return Json(new { success = true });
    }
    private void saveToAvatar(string imgbase64string, string username)
    {
      var base64string = "";
      var avatarPath = Path.Combine(this._webHostEnvironment.WebRootPath, $"img\\avatars\\{username}.png");
      if (imgbase64string.Contains("data:image"))
      {
        base64string = imgbase64string.Substring(imgbase64string.LastIndexOf(',') + 1);
      }
      else
      {
        base64string = imgbase64string;
      }
      var imageBytes = Convert.FromBase64String(base64string);
      using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
      {
        ms.Write(imageBytes, 0, imageBytes.Length);
        var image = System.Drawing.Image.FromStream(ms, true);
        image.Save(avatarPath, System.Drawing.Imaging.ImageFormat.Png);
      }

    }
  }
}
