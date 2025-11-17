using LMS.DTOs.Admin;
using LMS.DTOs.Employee;
using LMS.Model.Company;
using LMS.Model.Leave;
using LMS.ResponseModel;

namespace LMS.Repository
{
    public interface IAdminRepository
    {
        Task<EmployeeRegisteredLeaveAllocation?> AddEmployee(RegisterEmployee registerEmployee);
        Task<CompanyInfo?> GetCompanyIdByAdminId(int adminId);
        Task<bool> CheckIfExisting(string? email = null, int? Id = null);
        Task<List<GetEmployeeLeaveResponse>> GetEmployeeLeaveRequestsAsync(int adminId);
        Task<UpdateLeaveStatusResult> UpdateLeaveStatus(UpdateLeaveStatusRequest leaveStatus);
        Task<List<CompanyHoliday>> GetCompanyHolidays(int companyId);
        Task<LeaveTypes?> GetLeaveTypeAsync(int LeaveTypeId);
        Task<List<GetLeaveRequestResponse>> GetFilteredLeaveRequestsAsync(LeaveFilterRequest filter);
    }
}
