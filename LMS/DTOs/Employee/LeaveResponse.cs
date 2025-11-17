using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Employee
{
    public class LeaveResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class LeaveTypes
    {
        public int LeaveTypeId { get; set; }
        public string? LeaveType { get; set; }
        public string? GenderRestriction { get; set; }
        public int DefaultAllocation { get; set; }
        public string? AllocationFrequency { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class LeaveBalanceResponse
    {
        public int LeaveBalanceId { get; set; }
        public int LeaveTypeId { get; set; }
        public int EmployeeId { get; set; }
        public int TotalAllocatedDays { get; set; }
        public int TotalTakenDays { get; set; }
        public int RemainingDays { get; set; }
        public int Year { get; set; }
        public int CompanyId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

    }
}
