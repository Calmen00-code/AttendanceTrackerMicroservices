using AttendanceTrackerMicroservices.Models;

namespace AttendanceTrackerMicroservices.Service.IService
{
    public interface ITrackerService
    {
        Task<ResponseDTO?> ShouldUserCheckIn(string userId);
    }
}
