using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartAdmin.WebUI.Controllers.api
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReceiveCodeController : ControllerBase
  {
 
    // POST api/<ReceiveCodeController>
    [HttpPost]
    public IActionResult Post([FromBody] string code=null)
    {
      return Ok(new { code = 0, msg = "成功" });
    }

     
  }
}
