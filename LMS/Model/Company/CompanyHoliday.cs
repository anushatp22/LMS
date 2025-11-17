using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Company
{
    public class CompanyHoliday
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public string HolidayType { get; set; } // e.g., Public, Optional
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
