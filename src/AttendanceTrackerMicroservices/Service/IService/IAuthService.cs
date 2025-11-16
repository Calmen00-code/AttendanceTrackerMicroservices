using AttendanceTrackerMicroservices.Models;

namespace AttendanceTrackerMicroservices.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
        Task<ResponseDTO?> RegistrationAsync(RegistrationRequestDTO registrationRequest);
        Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO assignRoleRequest);
        Task<ResponseDTO?> ValidateUser(LoginRequestDTO validateRequest);
    }
}
