using AttendanceTrackerMicroservices.AuthAPI.Data;
using AttendanceTrackerMicroservices.AuthAPI.Models;
using AttendanceTrackerMicroservices.AuthAPI.Models.DTO;
using AttendanceTrackerMicroservices.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace AttendanceTrackerMicroservices.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
