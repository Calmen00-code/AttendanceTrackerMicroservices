using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;
using AttendanceTrackerMicroservices.Utility;
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

        public Task<ResponseDTO?> AssignRole(RegistrationRequestDTO assignRoleRequest)
        {
            throw new NotImplementedException();
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

        public Task<ResponseDTO?> RegistrationAsync(RegistrationRequestDTO registrationRequest)
        {
            throw new NotImplementedException();
        }
    }
}
