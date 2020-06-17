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
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
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
    [Route("login")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(jwtlogin login )
    {
      try
      {
        //Sign user in with username and password from parameters. This code assumes that the emailaddress is being used as the username. 
        var result = await _signInManager.PasswordSignInAsync(login.username, login.password, true, true);

        if (result.Succeeded)
        {
          //Retrieve authenticated user's details
          var user = await _userManager.FindByNameAsync(login.username);

          //Generate unique token with user's details
          var tokenString = await GenerateJSONWebToken(user);

          //Return Ok with token string as content
          return Ok(new { token = tokenString });
        }
        return Unauthorized();
      }
      catch (Exception e)
      {
        return StatusCode(500, e.Message);
      }
    }

    private async Task<string> GenerateJSONWebToken(ApplicationUser user)
    {
      //Hash Security Key Object from the JWT Key
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      //Generate list of claims with general and universally recommended claims
      var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                //添加自定义claim
                new Claim(ClaimTypes.GivenName, string.IsNullOrEmpty(user.GivenName) ? "" : user.GivenName),
                new Claim(ClaimTypes.Email, user.Email),
                 new Claim("http://schemas.microsoft.com/identity/claims/tenantid", user.TenantId.ToString()),
                  new Claim("http://schemas.microsoft.com/identity/claims/avatars", string.IsNullOrEmpty(user.Avatars) ? "" : user.Avatars),
                   new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)

    };
      //Retreive roles for user and add them to the claims listing
      var roles = await _userManager.GetRolesAsync(user);
      claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
      //Generate final token adding Issuer and Subscriber data, claims, expriation time and Key
      var token = new JwtSecurityToken(_config["Jwt:Issuer"]
          , _config["Jwt:Issuer"],
          claims,
          null,
          expires: DateTime.Now.AddDays(30),
          signingCredentials: credentials
      );

      //Return token string
      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
