using LMS.DTOs.Employee;
using LMS.ResponseModel;

namespace LMS.Interface
{
    public interface ILeaveService
    {
        Task AllocateAnnualLeaveAsync();
        Task AllocateMonthlyLeavesAsync();
        Task<bool> AllocateNewEmployeeLeave(EmployeeRegisteredLeaveAllocation employeeRegisteredLeaveAllocation);
        Task<bool> AddLeaveRequest(DTOs.Employee.LeaveRequest leaveRequest);
        Task<List<GetLeaveTypes>> GetLeaveTypes(int userID);
    }
}
