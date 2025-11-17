using LMS.DTOs.Employee;

namespace LMS.Interface
{
    public interface IJWTService
    {
        Task<string> GenerateToken(LoginConfirmJWTResponse employeeData, LoginRequest? loginRequest = null);
        //RefreshToken GenerateRefreshToken(string? userId = null);
    }
}
