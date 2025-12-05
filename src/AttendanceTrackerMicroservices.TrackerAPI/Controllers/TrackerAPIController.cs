using AttendanceTrackerMicroservices.TrackerAPI.Models;
using AttendanceTrackerMicroservices.TrackerAPI.Models.DTO;
using AttendanceTrackerMicroservices.TrackerAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QRCoder;

namespace AttendanceTrackerMicroservices.TrackerAPI.Controllers
{
    [Route("api/tracker")]
    [ApiController]
    public class TrackerAPIController : ControllerBase
    {
        private readonly ITrackerService _trackerService;
        protected ResponseDTO _response;

        public TrackerAPIController(ITrackerService trackerService)
        {
            _trackerService = trackerService;
            _response = new();
        }

        [HttpGet("should-user-check-in/{id}")]
        public async Task<bool> ShouldUserCheckIn(string? id)
        {
            Task<List<DailyAttendanceRecord>> dailyAttendanceRecordsTask = _trackerService.GetDailyAttendanceRecords(id);
            List<DailyAttendanceRecord> dailyAttendanceRecords =  await dailyAttendanceRecordsTask;

            // User have yet to check in for the day. So returning
            // true here to indicate first check in of the day.
            if (dailyAttendanceRecords.IsNullOrEmpty())
            {
                return true;
            }

            // When enter here, it means the user has already checked in today
            // Search if there are any records pending for check out. Otherwise, this is a new check-in request
            foreach (var record in dailyAttendanceRecords)
            {
                if (record.CheckOut == DateTime.MinValue)
                {
                    // Invalid check out record indicating that there is a pending check out
                    // So abort the check-in process
                    return true;
                }
            }
            return false;
        }

        [HttpGet("get-all-records/{id}")]
        public async Task<IActionResult> GetAllAttendanceRecords(string? id)
        {
            throw new Exception();
        }
    }
}
