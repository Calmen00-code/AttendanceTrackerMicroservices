using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AttendanceTrackerMicroservices.Models.ViewModels
{
    public class AuthenticationVM
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [ValidateNever]
        public string UserId { get; set; }

        [ValidateNever]
        public bool IsCheckIn { get; set; }

        public string Token { get; set; }
    }
}
