using LMS.DTOs.Common;
using LMS.DTOs.Employee;
using LMS.Model.Common;

namespace LMS.Repository
{
    public interface IEmployeeRepository
    {
        Task<LoginConfirmJWTResponse?> ConfirmUser(string email, string password);
        Task<bool> ResetPassword(int employeeId, string oldPassword, string newPassword);
        Task<int?> GetCompanyIdByEmployeeId(int employeeId);
        Task<string?> GetLeaveRequest(int LeaveApplicationId);
        Task<bool> CancelLeaveRequest(int CancelApplicationId);
        Task<List<GetLeaveRequestResponse>> GetLeaveRequestsForEmp(int userId, int? Id);
        Task<List<GetLeaveBalanceForEmployeeResponse>> GetLeaveBalance(int UserId);
        Task<LoginConfirmJWTResponse?> GetUserById(string userId);
        Task<IEnumerable<Notifications>> GetNotificationsByUserAsync(int userId);
        Task<bool> AddNotificationAsync(NotificationResponse notification);
        Task<Notifications> GetByIdAsync(Guid id);
        Task UpdateAsync(Notifications notification);
        Task<string> GetGenderById(int userID);
        Task<string?> GetEmailByEmployeeId(int employeeId);
    }
}
