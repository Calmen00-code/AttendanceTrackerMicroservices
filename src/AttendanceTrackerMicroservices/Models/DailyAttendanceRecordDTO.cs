using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerMicroservices.Models
{
    /// <summary>
    /// Represents a daily attendance record for an user,
    /// including check-in and check-out times on a specific single day.
    /// </summary>
    public class DailyAttendanceRecordDTO
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; } = DateTime.MinValue;

        public DateTime CheckOut { get; set; } = DateTime.MinValue;

        [Required]
        public string UserId { get; set; }
    }
}
