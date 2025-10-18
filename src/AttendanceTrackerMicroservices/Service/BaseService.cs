using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;

namespace AttendanceTrackerMicroservices.Service
{
    public class BaseService : IBaseService
    {
        public Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            throw new NotImplementedException();
        }
    }
}
