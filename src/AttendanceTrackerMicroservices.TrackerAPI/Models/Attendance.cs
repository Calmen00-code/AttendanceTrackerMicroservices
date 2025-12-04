using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceTrackerMicroservices.TrackerAPI.Models
{
    /// <summary>
    /// Represents the total working hours of an user for the entire day
    /// </summary>
    public class Attendance
    {
        [Key]
        public string Id { get; set; }

        public double TotalWorkingHours { get; set; }

        [Required]
        public DateTime RecordDate { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
