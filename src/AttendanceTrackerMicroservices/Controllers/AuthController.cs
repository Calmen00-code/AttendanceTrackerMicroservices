using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;
using AttendanceTrackerMicroservices.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AttendanceTrackerMicroservices.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

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
            ResponseDTO response = await _authService.LoginAsync(loginRequest);

            if (response != null && response.IsSuccess)
            {
                LoginResponseDTO loginResponse =
                    JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

                // setting up JWT token here after the first time login successfully
                // token that will be sent to server for the next API request will be setup here
                await SignInUser(loginResponse);
                _tokenProvider.SetToken(loginResponse.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                return View(loginRequest);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.RoleList = BuildDisplayRoleList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequest)
        {
            ResponseDTO response = await _authService.RegistrationAsync(registrationRequest);
            ResponseDTO assignRole;

            if (response != null && response.IsSuccess)
            {
                // at this point, user has been registered sucessfully

                // now check if user did not select any role
                // if so, default the role to Customer
                if (string.IsNullOrEmpty(registrationRequest.Role))
                {
                    registrationRequest.Role = SD.ROLE_CUSTOMER;
                }

                assignRole = await _authService.AssignRoleAsync(registrationRequest);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    // successfully assign role
                    TempData["success"] = "Registration successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = response.Message;
            }

            ViewBag.RoleList = BuildDisplayRoleList();
            return View();
        }

        // PRIVATE METHODS
        private List<SelectListItem> BuildDisplayRoleList()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem() { Text=SD.ROLE_ADMIN, Value = SD.ROLE_ADMIN },
                new SelectListItem() { Text=SD.ROLE_CUSTOMER, Value = SD.ROLE_CUSTOMER }
            };
            return roleList;
        }

        private async Task SignInUser(LoginResponseDTO login)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(login.Token);

            // Create identity with all JWT claims
            var identity = new ClaimsIdentity(jwt.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Add additional standard claims if needed for compatibility
            var email = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)?.Value;
            var role = jwt.Claims.FirstOrDefault(u => u.Type == "role")?.Value;

            if (email != null && !identity.HasClaim(ClaimTypes.Name, email))
                identity.AddClaim(new Claim(ClaimTypes.Name, email));

            if (role != null && !identity.HasClaim(ClaimTypes.Role, role))
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            var principal = new ClaimsPrincipal(identity);

            //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            //identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            //identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value));
            //identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            //identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            //identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            //var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
