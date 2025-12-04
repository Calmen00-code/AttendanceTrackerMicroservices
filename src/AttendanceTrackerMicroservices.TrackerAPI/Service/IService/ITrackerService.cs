using AttendanceTrackerMicroservices.TrackerAPI.Models;

namespace AttendanceTrackerMicroservices.TrackerAPI.Service.IService
{
    public interface ITrackerService
    {
        Task<List<DailyAttendanceRecord>> GetDailyAttendanceRecords(string userId);
    }
}
