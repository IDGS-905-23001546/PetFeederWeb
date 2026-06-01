using Microsoft.AspNetCore.Mvc;

namespace PawFeeder.Controllers
{
    public class AppController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Pets() => View();

        public IActionResult Schedules() => View();

        public IActionResult Device() => View();

        public IActionResult Profile() => View();
    }
}
