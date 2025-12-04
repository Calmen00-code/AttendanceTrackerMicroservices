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

        public async Task<List<DailyAttendanceRecord>> GetDailyAttendanceRecords(string userId)
        {
            return await _db.DailyAttendanceRecords
                .Where(u => u.UserId.ToLower() == userId).ToListAsync();
        }
    }
}
