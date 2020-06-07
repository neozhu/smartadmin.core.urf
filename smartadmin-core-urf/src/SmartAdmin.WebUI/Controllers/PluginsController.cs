using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class PluginsController : Controller
    {
        public IActionResult Appcore() => View();
        public IActionResult Bootbox() => View();
        public IActionResult Faq() => View();
        public IActionResult I18next() => View();
        public IActionResult Navigation() => View();
        public IActionResult Pacejs() => View();
        public IActionResult Slimscroll() => View();
        public IActionResult Smartpanels() => View();
        public IActionResult Throttle() => View();
        public IActionResult Waves() => View();
    }
}
