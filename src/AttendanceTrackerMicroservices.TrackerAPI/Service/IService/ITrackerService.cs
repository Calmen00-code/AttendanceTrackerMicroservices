using AttendanceTrackerMicroservices.TrackerAPI.Models;

namespace AttendanceTrackerMicroservices.TrackerAPI.Service.IService
{
    public interface ITrackerService
    {
        Task<List<DailyAttendanceRecord>> GetDailyAttendanceRecordsAsync(string userId);
        Task<string> CheckInAsync(DailyAttendanceRecord record);
        Task<string> CheckOutAsync(DailyAttendanceRecord record);
        Task<DailyAttendanceRecord?> FindCheckOutRecordFromRequest(DailyAttendanceRecordDTO request);
    }
}
