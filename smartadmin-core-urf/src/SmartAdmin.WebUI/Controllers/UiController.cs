using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class UiController : Controller
    {
        public IActionResult Accordion() => View();
        public IActionResult Alerts() => View();
        public IActionResult Badges() => View();
        public IActionResult Breadcrumbs() => View();
        public IActionResult ButtonGroup() => View();
        public IActionResult Buttons() => View();
        public IActionResult Cards() => View();
        public IActionResult Carousel() => View();
        public IActionResult Collapse() => View();
        public IActionResult Dropdowns() => View();
        public IActionResult ListFilter() => View();
        public IActionResult Modal() => View();
        public IActionResult Navbars() => View();
        public IActionResult Pagination() => View();
        public IActionResult Panels() => View();
        public IActionResult Popovers() => View();
        public IActionResult ProgressBars() => View();
        public IActionResult Scrollspy() => View();
        public IActionResult SidePanel() => View();
        public IActionResult Spinners() => View();
        public IActionResult TabsAccordions() => View();
        public IActionResult TabsPills() => View();
        public IActionResult Toasts() => View();
        public IActionResult Tooltips() => View();
        public IActionResult TooltipsPopovers() => View();
    }
}
