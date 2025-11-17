using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Employee
{
    public class LoginRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
