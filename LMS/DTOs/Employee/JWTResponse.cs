namespace LMS.DTOs.Employee
{
    public class JWTResponse
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }

    //to include employee details in the JWT response
    public class LoginConfirmJWTResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        //public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string Department { get; set; }
        public string EmployeeId { get; set; }
        public bool ForcePasswordChange { get; set; }
    }

    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
    }

}
