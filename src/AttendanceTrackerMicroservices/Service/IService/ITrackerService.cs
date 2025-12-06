using AttendanceTrackerMicroservices.Models;

namespace AttendanceTrackerMicroservices.Service.IService
{
    public interface ITrackerService
    {
        Task<ResponseDTO?> ShouldUserCheckInAsync(string userId);
        Task<ResponseDTO?> GetUserAttendanceRecordsForTodayAsync(string userId);
    }
}
