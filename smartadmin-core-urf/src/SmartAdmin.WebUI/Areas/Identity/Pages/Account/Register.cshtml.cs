using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class RegisterModel : PageModel
  {
    private readonly IEmailSender _emailSender;
    private readonly ILogger<RegisterModel> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public RegisterModel(
      IWebHostEnvironment webHostEnvironment,
      ApplicationDbContext dbContext,
      UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _logger = logger;
      _emailSender = emailSender;
      _dbContext = dbContext;
      _webHostEnvironment = webHostEnvironment;
    }

    [BindProperty] public InputModel Input { get; set; }
    public string Avatar{get;set;}

    public List<Tenant> Tenants { get; set; }
    public string ReturnUrl { get; set; }

    public void OnGet(string returnUrl = null)
    {
      Avatar = "avatar-lg.jpg";
      ReturnUrl = returnUrl;
      Tenants = _dbContext.Tenants.ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      returnUrl = returnUrl ?? Url.Content("~/");
      if (ModelState.IsValid)
      {
        var base64str = Input.Avatar;
        if (!string.IsNullOrEmpty(base64str))
        {
          this.saveToAvatar(base64str, Input.UserName);
          Input.Avatar = $"{Input.UserName}.png";
        }
        else
        {
          Input.Avatar = "avatar-lg.jpg";
        }
        var tenant = await this._dbContext.Tenants.FindAsync(Input.TenantId);
        var user = new ApplicationUser
        {
          UserName = Input.UserName,
          TenantDb = tenant.ConnectionStrings,
          TenantName = tenant.Name,
          TenantId = Input.TenantId,
          Email = Input.Email,
          PhoneNumber = Input.PhoneNumber,
          AvatarUrl = Input.Avatar,
          GivenName = Input.GivenName,
          EnabledChat = false

        };
        var result = await this._userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
          this._logger.LogInformation($"{Input.UserName}:注册成功");
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", user.TenantId.ToString()));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName,user.GivenName??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantname", user.TenantName??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantdb", user.TenantDb??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatarurl",  user.AvatarUrl ?? ""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, user.PhoneNumber ?? ""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.HomePhone,  user.PhoneNumber ?? ""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.OtherPhone, user.PhoneNumber ?? ""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Locality, "zh-cn"));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Country, "china"));
          await _signInManager.SignInAsync(user, true);
          return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      // If we got this far, something failed, redisplay form
      return Page();
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

    public class InputModel
    {
      [Display(Name = "用户名", Description = "用户名", Prompt = "登录用户名")]
      [Required]
      [RegularExpression(@"^[\u4E00-\u9FA5a-zA-Z0-9_]{2,20}$",ErrorMessage = "用户名不合法(字母开头，允许2-20位，允许字母数字下划线)")]
      public string UserName { get; set; }

      [Display(Name = "昵称", Description = "昵称", Prompt = "登录后显示的名称")]
      [RegularExpression(@"^[\u4E00-\u9FA5a-zA-Z0-9_]{2,20}$",ErrorMessage = "昵称不合法(字母开头，允许2-20位，允许字母数字下划线)")]
      public string GivenName { get; set; }

      [Required]
      [EmailAddress]
      [Display(Name = "邮件地址")]
      public string Email { get; set; }
      [Display(Name = "租户")]
      public int TenantId { get; set; }
      [Display(Name = "移动电话")]
      public string PhoneNumber { get; set; }

      [Required]
      [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
      [DataType(DataType.Password)]
      [Display(Name = "密码")]
      public string Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "确认密码")]
      [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
      public string ConfirmPassword { get; set; }

      [Required]
      [Display(Name = "我同意条款及细则")]
      public bool AgreeToTerms { get; set; }

      [Display(Name = "注册")]
      public bool SignUp { get; set; }

      public string Avatar { get; set; }
    }
  }
}
