using AttendanceTrackerMicroservices.Models;

namespace AttendanceTrackerMicroservices.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true);
    }
}
