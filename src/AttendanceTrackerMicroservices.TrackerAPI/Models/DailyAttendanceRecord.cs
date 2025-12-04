using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceTrackerMicroservices.TrackerAPI.Models
{
    /// <summary>
    /// Represents a daily attendance record for an user,
    /// including check-in and check-out times on a specific single day.
    /// </summary>
    public class DailyAttendanceRecord
    {
        [Key]
        public string Id { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
