using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Manual() => View();

        public IActionResult Contact() => View();

        [HttpPost]
        public IActionResult Contact(string nombre, string email, string asunto, string mensaje)
        {
            TempData["Success"] = "Mensaje enviado. Te responderemos pronto.";
            return RedirectToAction(nameof(Contact));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
