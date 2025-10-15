using AttendanceTrackerMicroservices.AuthAPI.Models.DTO;

namespace AttendanceTrackerMicroservices.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}
