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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Data.Models;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;

namespace SmartAdmin.WebUI.EndPoints
{
  [ApiController]
  [Route("api/users")]
  public class UsersEndpoint : ControllerBase
  {
    private readonly ILogger<UsersEndpoint> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _manager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly SmartSettings _settings;
    private readonly IConfiguration _config;

    public UsersEndpoint(ApplicationDbContext context,
      UserManager<ApplicationUser> manager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<UsersEndpoint> logger,
      IConfiguration config,
      SmartSettings settings)
    {
      _context = context;
      _manager = manager;
      _settings = settings;
      _signInManager = signInManager;
      _logger = logger;
      _config = config;
    }
    [Route("login")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(jwtlogin login)
    {
      try
      {
        //Sign user in with username and password from parameters. This code assumes that the emailaddress is being used as the username. 
        var result = await _signInManager.PasswordSignInAsync(login.username, login.password, true, true);

        if (result.Succeeded)
        {
          //Retrieve authenticated user's details
          var user = await _manager.FindByNameAsync(login.username);

          //Generate unique token with user's details
          var tokenString = await GenerateJSONWebToken(user);

          //Return Ok with token string as content
          _logger.LogInformation($"{login.username}:JWT登录成功");
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
      var roles = await _manager.GetRolesAsync(user);
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
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> Get()
    {
      var users = await _manager.Users.AsNoTracking().ToListAsync();

      return Ok(new { data = users, recordsTotal = users.Count, recordsFiltered = users.Count });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApplicationUser>> Get([FromRoute] string id) => Ok(await _manager.FindByIdAsync(id));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromForm] ApplicationUser model)
    {
      model.Id = Guid.NewGuid().ToString();
      model.UserName = model.Email;

      var result = await _manager.CreateAsync(model);

      if (result.Succeeded)
      {
        // HACK: This password is just for demonstration purposes!
        // Please do NOT keep it as-is for your own project!
        result = await _manager.AddPasswordAsync(model, "Password123!");

        if (result.Succeeded)
        {
          return CreatedAtAction("Get", new { id = model.Id }, model);
        }
      }

      return BadRequest(result);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromForm] ApplicationUser model)
    {
      var result = await _context.UpdateAsync(model, model.Id);

      if (result.Succeeded)
      {
        return NoContent();
      }

      return BadRequest(result);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromForm] ApplicationUser model)
    {
      // HACK: The code below is just for demonstration purposes!
      // Please use a different method of preventing the currently logged in user from being removed
      if (model.UserName == _settings.Theme.Email)
      {
        return BadRequest(SmartError.Failed("Please do not delete the main user! =)"));
      }

      var result = await _context.DeleteAsync<ApplicationUser>(model.Id);

      if (result.Succeeded)
      {
        return NoContent();
      }

      return BadRequest(result);
    }
  }
}
