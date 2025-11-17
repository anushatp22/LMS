namespace LMS.DTOs.Admin
{
    public class LeaveFilterRequest
    {
            public LeaveStatus? Status { get; set; }  // Optional filter
            public DateTime? StartDate { get; set; }  // Optional filter
            public DateTime? EndDate { get; set; }    // Optional filter
       
    }
}
