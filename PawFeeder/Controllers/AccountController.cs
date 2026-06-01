using Microsoft.AspNetCore.Mvc;

namespace PawFeeder.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            return RedirectToAction("Index", "App");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string nombre, string apellido, string email, string password, string confirmPassword)
        {
            return RedirectToAction("Index", "App");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
