using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccessDeniedModel> _logger;

    public AccessDeniedModel(SignInManager<ApplicationUser> signInManager, ILogger<AccessDeniedModel> logger)
    {
      _signInManager = signInManager;
      _logger = logger;
    }

    public async Task OnGet()
    {
      await _signInManager.SignOutAsync();

      _logger.LogInformation("User logged out.");
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
      await _signInManager.SignOutAsync();

      _logger.LogInformation("User logged out.");

      if (returnUrl != null)
      {
        return LocalRedirect(returnUrl);
      }
      else
      {
        return Page();
      }
    }
  }
}

