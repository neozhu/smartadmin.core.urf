using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    [AllowAnonymous]
    public class DocumentationController : Controller
    {
        public IActionResult Introduction() => View();
        public IActionResult GettingStarted() => View();
        public IActionResult SiteStructure() => View();
        public IActionResult SolutionArchitecture() => View();
        public IActionResult Customizations() => View();
        public IActionResult HowtoContribute() => View();
        public IActionResult LicensingInformation() => View();
        public IActionResult Changelog() => View();
    }
}
