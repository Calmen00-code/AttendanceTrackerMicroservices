using AttendanceTrackerMicroservices.TrackerAPI.Data;
using AttendanceTrackerMicroservices.TrackerAPI.Models;
using AttendanceTrackerMicroservices.TrackerAPI.Service.IService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AttendanceTrackerMicroservices.TrackerAPI.Service
{
    public class TrackerService : ITrackerService
    {
        private readonly AppDbContext _db;

        public TrackerService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<DailyAttendanceRecord>> GetDailyAttendanceRecordsAsync(string userId)
        {
            return await _db.DailyAttendanceRecords
                .Where(u => u.UserId.ToLower() == userId && u.CheckIn.Date == DateTime.Today)
                .ToListAsync();
        }

        public async Task<string> AddNewDailyAttendanceRecordAsync(DailyAttendanceRecord record)
        {
            try
            {
                await _db.DailyAttendanceRecords.AddAsync(record);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
    }
}
