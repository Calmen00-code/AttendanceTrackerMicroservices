using AttendanceTrackerMicroservices.Models;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceTrackerMicroservices.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new LoginRequestDTO();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            return Ok();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
