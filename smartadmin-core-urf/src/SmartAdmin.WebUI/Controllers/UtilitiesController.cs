using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class UtilitiesController : Controller
    {
        public IActionResult Borders() => View();
        public IActionResult Clearfix() => View();
        public IActionResult ColorPallet() => View();
        public IActionResult DisplayProperty() => View();
        public IActionResult Flexbox() => View();
        public IActionResult Fonts() => View();
        public IActionResult Helpers() => View();
        public IActionResult Position() => View();
        public IActionResult ResponsiveGrid() => View();
        public IActionResult Sizing() => View();
        public IActionResult Spacing() => View();
        public IActionResult Typography() => View();
    }
}
