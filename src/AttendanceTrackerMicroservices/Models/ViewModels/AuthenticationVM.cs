using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerMicroservices.Models.ViewModels
{
    public class AuthenticationVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [ValidateNever]
        public string EmployeeId { get; set; }

        [ValidateNever]
        public bool IsCheckIn { get; set; }

        public string Token { get; set; }
    }
}
