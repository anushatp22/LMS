using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Leave
{
    public class LeaveApplication
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // e.g., Pending, Approved, Rejected
        public DateTime AppliedAt { get; set; }
        public DateTime? DateApproved { get; set; }
        public int? ReviewedBy { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
