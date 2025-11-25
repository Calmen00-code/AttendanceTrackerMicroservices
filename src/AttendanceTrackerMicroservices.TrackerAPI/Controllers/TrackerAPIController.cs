using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace AttendanceTrackerMicroservices.TrackerAPI.Controllers
{
    [Route("api/tracker")]
    [ApiController]
    public class TrackerAPIController : ControllerBase
    {
        [HttpGet("get-all-records/{id}")]
        public async Task<IActionResult> GetAllAttendanceRecords(string? id)
        {
            throw new Exception();
        }
    }
}
