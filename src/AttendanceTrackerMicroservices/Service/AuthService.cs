using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;

namespace AttendanceTrackerMicroservices.Service
{
    public class AuthService : IAuthService
    {
        public Task<ResponseDTO?> AssignRole(RegistrationRequestDTO assignRoleRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO?> RegistrationAsync(RegistrationRequestDTO registrationRequest)
        {
            throw new NotImplementedException();
        }
    }
}
