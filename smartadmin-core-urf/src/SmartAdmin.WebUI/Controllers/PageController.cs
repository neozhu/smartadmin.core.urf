using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class PageController : Controller
    {
       
        public IActionResult Error() => View();
        public IActionResult Error404() => View();
      
    }
}
