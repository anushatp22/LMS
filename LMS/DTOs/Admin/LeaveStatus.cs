using LMS.Model.Leave;

namespace LMS.DTOs.Admin
{
    public enum LeaveStatus
    {
      Pending, // Default when applied
      Approved,
      Rejected
    }
    public class UpdateLeaveStatusRequest
    {
        public int LeaveId { get; set; }
        public LeaveStatus NewStatus { get; set; }
        public int ReviewedBy { get; set; }
    }


    public class UpdateLeaveStatusResult
    {
        public int RowsAffected { get; set; }
        public LeaveApplication? UpdatedLeave { get; set; }
    }

}
