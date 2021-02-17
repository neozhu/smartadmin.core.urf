using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Controllers
{
  [Authorize]
  public class AccountManageController : Controller
  {
    private readonly ILogger<AccountManageController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public AccountManageController(
      IWebHostEnvironment webHostEnvironment,
              ILogger<AccountManageController> logger,
              UserManager<ApplicationUser> userManager,
              ApplicationDbContext dbContext,
              RoleManager<IdentityRole> roleManager,
              SignInManager<ApplicationUser> signInManager

                               ) {
      _webHostEnvironment = webHostEnvironment;
      _logger = logger;
      _userManager = userManager;
      _dbContext = dbContext;
      _roleManager = roleManager;
      _signInManager = signInManager;
    }
     
 
    public async Task<IActionResult> Index() {
      var selectlist = await this._dbContext.Tenants
        .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
        .ToListAsync();
      ViewBag.TenantId = selectlist;
      return View();
      }
    //租户管理
    public IActionResult Tenant() => View();

    //获取租户数据
    public async Task<IActionResult> GetTenantData()
    {
      var data = await this._dbContext.Tenants.ToListAsync();
      return Json(data);
    }

    //解锁，加锁账号
    public async Task<JsonResult> SetLockout(string[] userid)
    {
      foreach (var id in userid)
      {
        var user =await this._userManager.FindByIdAsync(id);
        await this._userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddDays(30)));
      }
      return Json(new { success = true });
    }
    //注册新用户
    public async Task<JsonResult> ReregisterUser(AccountRegistrationModel model) {
      if (this.ModelState.IsValid)
      {
        var tenant =await this._dbContext.Tenants.FindAsync(model.TenantId);
        this.saveToAvatar(model.Avatar, model.Username);
        var user = new ApplicationUser
        {
          UserName = model.Username,
          TenantDb = tenant.ConnectionStrings,
          TenantName = tenant.Name,
          TenantId = model.TenantId,
          Email = model.Email,
          PhoneNumber = model.PhoneNumber,
          AvatarUrl = $"{model.Username}.png",
          GivenName = model.GivenName,
          EnabledChat = false

        };
        var result = await this._userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          this._logger.LogInformation($"{user.UserName}:注册成功");
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", user.TenantId.ToString()));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName,  user.GivenName??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantname", user.TenantName??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantdb",  user.TenantDb??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatarurl", user.AvatarUrl??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone,   user.PhoneNumber??""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.HomePhone,   user.PhoneNumber ?? ""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.OtherPhone,  user.PhoneNumber ?? ""));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Locality, "zh-cn"));
          await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Country, "china"));
          var role = "users";
          var any = await this._roleManager.FindByNameAsync(role);
          if (any != null)
          {
            await this._userManager.AddToRoleAsync(user, role);
          }
          else {
            await  _roleManager.CreateAsync(new IdentityRole() { Name = role });
            await _userManager.AddToRoleAsync(user, role);
          }
          return Json(new { success = true });
        }
        else
        {
          return Json(new { success = false, err = string.Join(",", result.Errors) });
        }

      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
      }
    }

    //修改账号
    [HttpPost]
    public async Task<JsonResult> UpdateUser(AccountUpdateModel model)
    {
      var tenant = await this._dbContext.Tenants.FindAsync(model.TenantId);
      var user = this._dbContext.Users.Find(model.Id);

      this.saveToAvatar(model.Avatar, user.UserName);
      user.GivenName = model.GivenName;
      user.Email = model.Email;
      user.PhoneNumber = model.PhoneNumber;
      user.TenantId = model.TenantId;
      user.TenantName = tenant.Name;
      user.TenantDb = tenant.ConnectionStrings;
      user.AvatarUrl = $"{user.UserName}.png";


      await this._dbContext.SaveChangesAsync();
      var clamins = await this._userManager.GetClaimsAsync(user);
      var tenantclamin = clamins.Where(x => x.Type == "http://schemas.microsoft.com/identity/claims/tenantid").FirstOrDefault();
      if (tenantclamin != null)
      {
        await this._userManager.RemoveClaimAsync(user, tenantclamin);
      }
      var emailclamin = clamins.Where(x => x.Type == System.Security.Claims.ClaimTypes.Email).FirstOrDefault();
      if (emailclamin != null)
      {
        await this._userManager.RemoveClaimAsync(user, emailclamin);
      }
      var phoneclamin = clamins.Where(x => x.Type == System.Security.Claims.ClaimTypes.MobilePhone).FirstOrDefault();
      if (phoneclamin != null)
      {
        await this._userManager.RemoveClaimAsync(user, phoneclamin);
      }
      var nameclamin = clamins.Where(x => x.Type == System.Security.Claims.ClaimTypes.GivenName).FirstOrDefault();
      if (nameclamin != null)
      {
        await this._userManager.RemoveClaimAsync(user, nameclamin);
      }
      var avatarclamin = clamins.Where(x => x.Type == "http://schemas.microsoft.com/identity/claims/avatarurl").FirstOrDefault();
      if (avatarclamin != null)
      {
        await this._userManager.RemoveClaimAsync(user, avatarclamin);
      }

      await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", user.TenantId.ToString()));
      await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, user.GivenName ?? ""));
      await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email));
      await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, user.PhoneNumber ?? ""));
      await this._userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatarurl", user.AvatarUrl ?? ""));
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
    public async Task<JsonResult> SetUnLockout(string[] userid)
    {
      foreach (var id in userid)
      {
        var user = await this._userManager.FindByIdAsync(id);
        await this._userManager.SetLockoutEndDateAsync(user,new DateTimeOffset(DateTime.Now));
      }
      return Json(new { success = true });
    }
    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<JsonResult> ResetPassword(string id, string newPassword)
    {
      var user = await this._userManager.FindByIdAsync(id);
      var code =await this._userManager.GeneratePasswordResetTokenAsync(user);
      var result =await this._userManager.ResetPasswordAsync(user, code, newPassword);
      if (result.Succeeded)
      {
        return Json(new { success = true });
      }
      else
      {
        return Json(new { success = false, err = string.Join(",", result.Errors) });
      }

    }
    //保存租户信息
    public async Task<JsonResult> SaveTenantData(Tenant[] tenant) {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in tenant)
          {
            if(item.TrackingState== 1)
            {
              this._dbContext.Tenants.Add(item);
            }
            if (item.TrackingState == 2)
            {
              var update = await this._dbContext.Tenants.Where(x => x.Id == item.Id).FirstAsync();
              update.Name = item.Name;
              update.Description = item.Description;
              update.ConnectionStrings = item.ConnectionStrings;
              update.Disabled = item.Disabled;
              this._dbContext.Update(update);
             
            }
            if (item.TrackingState == 3)
            {
              var delete = await this._dbContext.Tenants.Where(x => x.Id == item.Id).FirstAsync();
              this._dbContext.Tenants.Remove(delete);

            }
           
          }
          await this._dbContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
      }

    }
    //删除租户信息
    public async Task<JsonResult> DeleteCheckedTenant(int[] id) {
      var items = this._dbContext.Tenants.Where(x => id.Contains(x.Id));
      foreach (var item in items)
      {
        this._dbContext.Tenants.Remove(item);
      }
      await this._dbContext.SaveChangesAsync();
      return Json(new { success = true });
    }
    [HttpGet]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<ApplicationUser>(filterRules);
      var totalCount = 0;

      var users = this._userManager.Users.Where(filters).OrderBy($"{sort}  {order}");
       
      totalCount =await users.CountAsync();
      var datalist =await users.Skip(( page - 1 ) * rows).Take(rows).ToListAsync();
      var datarows = datalist.Select(n => new
      {
        Id = n.Id,
        UserName = n.UserName,
        GivenName=n.GivenName,
        TenantDb = n.TenantName,
        TenantName = n.TenantName,
        Email = n.Email,
        TenantId = n.TenantId,
        PhoneNumber = n.PhoneNumber,
        AvatarUrl = n.AvatarUrl,
        AccessFailedCount = n.AccessFailedCount,
        LockoutEnabled = n.LockoutEnabled,
        LockoutEnd = n.LockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss"),
        IsOnline = n.IsOnline,
        EnabledChat = n.EnabledChat
      }).ToList();
      var pagelist = new { total = totalCount, rows = datarows };
      return this.Json(pagelist);
    }
    //获取租户数据
    [HttpGet]
    public async Task<JsonResult> GetTenantData(int page = 1, int rows = 10, string sort = "Id", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.FromFilter<Tenant>(filterRules);
      var totalCount = 0;

      var tenants = this._dbContext.Tenants.Where(filters).OrderBy($"{sort}  {order}");
       
      totalCount =await tenants.CountAsync();
      var pagerows =(await tenants.Skip(( page - 1 ) * rows).Take(rows).ToListAsync())
            .Select(n => new
      {
        Id = n.Id,
        Name = n.Name,
        n.Description,
        ConnectionStrings = n.ConnectionStrings
      }).ToList();
      var pagelist = new { total = totalCount, rows = pagerows };
      return this.Json(pagelist);
    }
     
    //删除用户
    [HttpPost]
    public async Task<JsonResult> DeleteCheckedUser(string[] id)
    {
      foreach (var key in id)
      {
        var user = await this._userManager.FindByIdAsync(key);
        await this._userManager.DeleteAsync(user);
      }
      return this.Json(new { success = true });
    }
    


  }
}
