using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Admin
{
    public class RegisterEmployee
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public int CompanyId { get; set; }
        public DateTime JoiningDate { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
    }
}
