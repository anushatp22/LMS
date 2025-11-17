using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Employee
{
    public class LeaveRequest
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
