using LMS.DTOs.Employee;
using LMS.Model.Leave;

namespace LMS.Repository
{
    public interface ILeaveRepository
    {
        Task<List<LeaveResponse>> GetAllActiveEmployeesAsync();
        Task<List<LeaveTypes>> GetLeaveTypesAsync(List<string> requestValue);
        Task<List<LeaveBalanceResponse>> GetLeaveBalancesAsync(int? employeeId = null,
    int? leaveTypeId = null, int? year = null);
        Task<bool> UpdateLeaveBalanceAsync(IEnumerable<LeaveBalanceResponse> leaveBalance);
        Task<bool> AddLeaveBalanceAsync(IEnumerable<LeaveBalanceResponse> leaveBalance);
        Task<bool> AddLeaveRequest(LeaveApplication requestValue);
        Task<List<GetLeaveTypes>> GetLeaveTypesAsync(string Gender);
    }
}
