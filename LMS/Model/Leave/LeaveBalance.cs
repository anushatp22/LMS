using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Leave
{
    public class LeaveBalance
    {
        [Key]
        public int Id { get; set; }
        public int LeaveTypeId { get; set; }
        public int EmployeeId { get; set; }
        public int TotalAllocatedDays { get; set; }
        public int TotalTakenDays { get; set; }
        public int RemainingDays { get; set; }
        public int Year { get; set; }
        public int CompanyId { get; set; }
        public DateTime? LastAllocatedDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
