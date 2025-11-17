namespace LMS.DTOs.Admin
{
    public class GetEmployeeLeaveResponse
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? ReviewedBy { get; set; }
    }
}
