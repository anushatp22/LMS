using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Admin
{
    public class EmployeeRegistration
    {
        private const string V = "Employee";

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Gender { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public string Department { get; set; }
        public string Role { get; set; } = V;
        public DateTime JoiningDate { get; set; }
        [Required]
        public string PasswordHash {  get; set; }
        public bool ForcePasswordChange { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } 
        public DateTime? LastUpdatedAt { get; set; }




    }
}
