using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class FormPluginsController : Controller
    {
        public IActionResult Colorpicker() => View();
        public IActionResult Datepicker() => View();
        public IActionResult DaterangePicker() => View();
        public IActionResult Dropzone() => View();
        public IActionResult Imagecropper() => View();
        public IActionResult Inputmask() => View();
        public IActionResult Ionrangeslider() => View();
        public IActionResult Markdown() => View();
        public IActionResult Nouislider() => View();
        public IActionResult Select2() => View();
        public IActionResult Summernote() => View();
        public IActionResult Wizard() => View();
    }
}
