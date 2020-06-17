using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    public AccountController(
      SignInManager<ApplicationUser> signInManager,
      UserManager<ApplicationUser> userManager,
      IConfiguration config,
      ILogger<AccountController> logger
      )
    {
      _logger = logger;
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
    }

    public IActionResult Profile() => View();
  }
}
