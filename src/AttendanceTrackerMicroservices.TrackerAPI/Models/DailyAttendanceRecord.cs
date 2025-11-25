using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceTrackerMicroservices.TrackerAPI.Models
{
    public class DailyAttendanceRecord
    {
        [Key]
        public string Id { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
