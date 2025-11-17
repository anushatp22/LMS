using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Employee
{
    public class PasswordResetRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
