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

            if (dailyAttendanceRecords.IsNullOrEmpty())
            {
                return true;
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
