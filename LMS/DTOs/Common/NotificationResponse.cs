namespace LMS.DTOs.Common
{
    public class NotificationResponse
    {
            public Guid? Id { get; set; }
            public int UserId { get; set; }
            public string Message { get; set; } = string.Empty;
            public string Type { get; set; } = "info"; // success, error, info, warning
            public bool IsRead { get; set; }
            public DateTime? CreatedAt { get; set; }
        
    }
}
