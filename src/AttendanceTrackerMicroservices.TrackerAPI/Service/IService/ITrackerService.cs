using AttendanceTrackerMicroservices.TrackerAPI.Models;

namespace AttendanceTrackerMicroservices.TrackerAPI.Service.IService
{
    public interface ITrackerService
    {
        Task<List<DailyAttendanceRecord>> GetDailyAttendanceRecordsAsync(string userId);
        Task<string> AddNewDailyAttendanceRecordAsync(DailyAttendanceRecord record);
    }
}
