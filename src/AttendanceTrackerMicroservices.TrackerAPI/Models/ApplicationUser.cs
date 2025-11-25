using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AttendanceTrackerMicroservices.TrackerAPI.Models
{
    public class ApplicationUser
    {
        [ValidateNever]
        public ICollection<Attendance> Attendances { get; set; }
    }
}
