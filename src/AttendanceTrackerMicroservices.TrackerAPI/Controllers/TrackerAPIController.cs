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
        public async Task<IActionResult> ShouldUserCheckIn(string? id)
        {
            var dailyAttendanceRecordsTask = _trackerService.GetDailyAttendanceRecordsAsync(id);
            List<DailyAttendanceRecord> dailyAttendanceRecords =  await dailyAttendanceRecordsTask;

            _response.IsSuccess = true;
            _response.Result = true;

            // User have yet to check in for the day. So returning
            // true here to indicate first check in of the day.
            if (dailyAttendanceRecords.IsNullOrEmpty())
            {
                return Ok(_response);
            }

            // When enter here, it means the user has already checked in today
            // Search if there are any records pending for check out. Otherwise, this is a new check-in request
            foreach (var record in dailyAttendanceRecords)
            {
                if (record.CheckOut == DateTime.MinValue)
                {
                    // Invalid check out record indicating that there is a pending check out
                    // So abort the check-in process
                    _response.Result = false;
                    return Ok(_response);
                }
            }

            return Ok(_response);
        }

        [HttpGet("get-today-attendance-records/{id}")]
        public async Task<IActionResult> GetTodayAttendanceRecords(string? id)
        {
            var dailyAttendanceRecordsTask = _trackerService.GetDailyAttendanceRecordsAsync(id);
            List<DailyAttendanceRecord> dailyAttendanceRecords =  await dailyAttendanceRecordsTask;

            // Mapping to DTO list
            List<DailyAttendanceRecordDTO> dailyAttendanceRecordsDTOList =
                dailyAttendanceRecords.Select(record => new DailyAttendanceRecordDTO()
                {
                    Id = record.Id,
                    CheckIn = record.CheckIn,
                    CheckOut = record.CheckOut,
                    UserId = record.UserId,
                }).ToList();

            return Ok(dailyAttendanceRecords);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> PerformCheckIn([FromBody] DailyAttendanceRecordDTO record)
        {
            DailyAttendanceRecord dbCheckInRecord = new DailyAttendanceRecord()
            {
                Id = record.CheckIn.ToString("yyyy-MM-dd") + "_" +
                     record.CheckIn.ToString("HH:mm") + "_" + record.UserId,
                CheckIn = record.CheckIn,
                UserId = record.UserId
            };

            string errorMessage = await _trackerService.CheckInAsync(dbCheckInRecord);
            if (!errorMessage.IsNullOrEmpty())
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;

                return BadRequest(_response);
            }

            return Ok(_response);
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> PerformCheckOut([FromBody] DailyAttendanceRecordDTO record)
        {
            // Get the correct dbCheckoutRecord that has VALID Id to perform checkout
            DailyAttendanceRecord? dbCheckoutRecord =
                _trackerService.FindCheckOutRecordFromRequest(record).GetAwaiter().GetResult();

            if (dbCheckoutRecord == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Unable to find checkout record";
                return BadRequest(_response);
            }

            // Now that we get the correct entry for checkout, update the checkout value
            dbCheckoutRecord.CheckOut = record.CheckOut;

            string errorMessage = await _trackerService.CheckOutAsync(dbCheckoutRecord);
            if (!errorMessage.IsNullOrEmpty())
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;

                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
