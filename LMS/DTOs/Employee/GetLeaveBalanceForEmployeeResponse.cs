namespace LMS.DTOs.Employee
{
    public class GetLeaveBalanceForEmployeeResponse
    {
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public int TotalTakenDays { get; set; }
        public int RemainingDays { get; set; }

    }
}
