using LMS.DTOs.Admin;
using LMS.ResponseModel.Common;
using LMS.ResponseModel;

namespace LMS.Interface
{
    public interface IAdminService
    {
        Task<ApiResponse<EmployeeRegistrationResult>> AddEmployee(RegisterEmployee registerEmployee);
        Task<ApiResponse<List<GetEmployeeLeaveResponse>>> GetEmployeeLeaveRequestsAsync(int adminId);
        Task<ApiResponse<object>> UpdateLeaveStatus(UpdateLeaveStatusRequest leaveStatus);
        Task<ApiResponse<object>> GetFilteredLeaveRequestsAsync(LeaveFilterRequest filter);
    }
}
