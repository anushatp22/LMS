namespace LMS.DTOs.Employee
{
    public class GetLeaveRequestResponse
    {
        public int LeaveRequestId { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? DateApproved {  get; set; }
    }
}
