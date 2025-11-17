using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Company
{
    public class CompanyAdmin
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool isActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
