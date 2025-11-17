namespace LMS.DTOs.Employee
{
    public class RefreshTokenPayload
    {
        public string UserId { get; set; }
        public DateTime Expires { get; set; }
    }
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }

}
