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

        public async Task<string> CheckInAsync(DailyAttendanceRecord record)
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

        public async Task<string> CheckOutAsync(DailyAttendanceRecord record)
        {
            // To find the check-out record, we need to find:
            //  1. UserID that help us to identify which user we are looking at
            //  2. The date of the check-in matches to the record passed in as parameter
            //     (this is because one user could have multiple record on the same day)
            //  3. And finally, the check-out value must be default value (DateTime.MinValue)
            DailyAttendanceRecord? checkOutRecord =
                await _db.DailyAttendanceRecords.FirstOrDefaultAsync(u =>
                    u.UserId == record.UserId &&
                    u.CheckIn.Date == record.CheckIn.Date &&
                    u.CheckOut == DateTime.MinValue);

            if (checkOutRecord == null)
            {
                return "Checkout record cannot be found!";
            }

            _db.Update(checkOutRecord);
            await _db.SaveChangesAsync();
            return string.Empty;
        }

        /// <summary>
        /// Helper method to find the correct checkout record from database
        /// </summary>
        /// <param name="request">Incoming checkout request from client</param>
        /// <returns>Returns the correct record back to the client API, so that correct record is used</returns>
        public async Task<DailyAttendanceRecord?> FindCheckOutRecordFromRequest(DailyAttendanceRecordDTO request)
        {
            // To find the correct checkout record:
            // 1. UserId must match
            // 2. There must be existing record in database with Check-In value
            //    same date as the request's Check-Out
            // 3. Since we do not checkout yet, then the existing checkout value must be DateTime.MinValue
            return await _db.DailyAttendanceRecords.FirstOrDefaultAsync(u =>
                u.UserId == request.UserId &&
                u.CheckIn.Date == request.CheckOut.Date &&
                u.CheckOut == DateTime.MinValue);
        }
    }
}
