using LMS.DTOs.Common;
using LMS.DTOs.Employee;
using LMS.ResponseModel.Common;

namespace LMS.Interface
{
    public interface IEmployeeService
    {
        Task<JwtLoginResponse<LoginTokens>> LoginUser(LoginRequest loginRequest);
        Task<ApiResponse<object>> PasswordResetRequest(int id, PasswordResetRequest passwordReset);
        Task<ApiResponse<object>> CancelLeaveRequest(int Id);
        Task<ApiResponse<object>> GetLeaveRequest(int UserId, int? id);
        Task<ApiResponse<object>> GetLeaveBalance(int UserId);
        Task<ApiResponse<object>> GetNotifications(int userId);
        Task<ApiResponse<object>> AddNotificationAsync(NotificationResponse notification);
        Task<NotificationResponse?> MarkAsReadAsync(Guid id);
    }
}
