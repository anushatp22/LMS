namespace LMS.ResponseModel.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }              // Indicates overall success
        public List<string> Messages { get; set; }     // Can hold success/failure/info messages
        public T? Data { get; set; }                   // The actual data (optional)

        public ApiResponse()
        {
            Messages = new List<string>();
        }
    }

    public class JwtLoginResponse<T>
    {
        public bool Success { get; set; }              // Indicates overall success
        public List<string> Messages { get; set; }     // Can hold success/failure/info messages
        public T? Data { get; set; }
    }
        // Example of the payload for login tokens
        public class LoginTokens
        {
            public string AccessToken { get; set; } = string.Empty;
        //public string RefreshToken { get; set; } = string.Empty;
        //public DateTime RefreshTokenExpiry { get; set; }
        public int? EmployeeId { get; set; }
        public bool? ForcePasswordChange { get; set; }
        public string? Email { get; set; }
    }

    //public class ForcePasswordChangeResponse
    //{
    //    public int EmployeeId { get; set; }
    //    public bool ForcePasswordChange { get; set; }
    //    public string Email { get; set; }
    //}
    }
