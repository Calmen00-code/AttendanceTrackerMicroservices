using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;
using AttendanceTrackerMicroservices.Utility;
using Microsoft.AspNetCore.Identity.Data;
using static AttendanceTrackerMicroservices.Utility.SD;

namespace AttendanceTrackerMicroservices.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO assignRoleRequest)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = assignRoleRequest,
                Url = AuthAPIBase + "/api/auth/AssignRole"
            });
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            RequestDTO request = new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = loginRequest,
                Url = AuthAPIBase + "/api/auth/login"
            };

            return await _baseService.SendAsync(request, withBearer: false);
        }

        public async Task<ResponseDTO?> RegistrationAsync(RegistrationRequestDTO registrationRequest)
        {
            RequestDTO request = new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = registrationRequest,
                Url = AuthAPIBase + "/api/auth/register"
            };

            return await _baseService.SendAsync(request, withBearer: false);
        }

        public async Task<ResponseDTO?> ValidateUser(LoginRequestDTO validateRequest)
        {
            RequestDTO request = new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = validateRequest,
                Url = AuthAPIBase + "/api/auth/validateUser"
            };

            return await _baseService.SendAsync(request, withBearer: false);
        }
    }
}
