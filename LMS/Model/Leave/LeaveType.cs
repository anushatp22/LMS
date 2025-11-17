using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Leave
{
    public class LeaveType
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        public string? GenderRestriction { get; set; }
        public int DefaultAllocation { get; set; }
        public string? AllocationFrequency { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
