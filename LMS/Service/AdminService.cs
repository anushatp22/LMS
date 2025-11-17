using LMS.Controllers;
using LMS.DTOs.Admin;
using LMS.DTOs.Employee;
using LMS.Interface;
using LMS.Model.Leave;
using LMS.Repository;
using LMS.ResponseModel;
using LMS.ResponseModel.Common;
using Microsoft.AspNetCore.Identity;

namespace LMS.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;
        private readonly ILeaveService _leaveService;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public AdminService(IAdminRepository adminRepository, IEmailService emailService, ILeaveService leaveService, ILeaveRepository leaveRepository,
             IEmployeeRepository employeeRepository)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
            _leaveService = leaveService;
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<ApiResponse<EmployeeRegistrationResult>> AddEmployee(RegisterEmployee registerEmployee)
        {
            var result = new ApiResponse<EmployeeRegistrationResult>
            {
                Data = new EmployeeRegistrationResult()
            };
            if (registerEmployee != null)
                try
                {
                    {
                        //check if email is already existing
                        var checkEmail = await _adminRepository.CheckIfExisting(registerEmployee.Email);
                        if (checkEmail == true)
                        {
                            result.Success = false;
                            result.Messages.Add("Email already exists.");
                            return result;
                        }
                        var hasher = new PasswordHasher<RegisterEmployee>();
                        var password = registerEmployee.PasswordHash;
                        registerEmployee.PasswordHash = hasher.HashPassword(registerEmployee, registerEmployee.PasswordHash);
                        //----------------------------------------------------------till real authentication will be done--------------------------------------------
                        //var adminId = User.FindFirst("Id")?.Value;
                        //var companyDetails = await _adminRepository.GetCompanyIdByAdminId(adminId);
                        var companyName = "TCS";
                        //if (companyDetails == null)
                        //{
                        //    result.Messages.Add("Company not found for the given admin to send Email.");
                        //    return result;
                        //}
                        //registerEmployee.CompanyId = companyDetails.companyId;
                        registerEmployee.CompanyId = 1;
                        var addEmployee = await _adminRepository.AddEmployee(registerEmployee);
                        if (!addEmployee.Success)
                        {
                            result.Messages.Add("Failed to register employee. Please try again.");
                            result.Success = false;
                            return result;
                        }
                        var allocateLeave = await _leaveService.AllocateNewEmployeeLeave(addEmployee);
                        if (!allocateLeave)
                        {
                            result.Messages.Add($"Failed to allocate leaves for new employee with employeeId: {addEmployee.EmployeeId}");
                            result.Success = false;
                        }
                        //try sending mail
                        try
                        {
                            if (addEmployee.Success == true && companyName != null)
                            {
                                await _emailService.SendTempPasswordAsync(registerEmployee.Email, password, companyName);
                                result.Data.EmailSent = true;
                                result.Messages!.Add("Employee registered and email sent successfully.");

                            }
                        }
                        catch (Exception ex)
                        {
                            result.Data.EmailSent = false;
                            result.Messages!.Add($"Employee registered but email sending failed. You can try sending with the credentials {registerEmployee.Email} & {password}");
                        }
                        result.Success = true;
                        result.Data.IsRegistered = true;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    result.Messages.Add($"An error occurred while registering the employee: {ex.Message}");
                    result.Success = false;
                    return result;
                }
            return result;
        }

        public async Task<ApiResponse<List<GetEmployeeLeaveResponse>>> GetEmployeeLeaveRequestsAsync(int adminId)
        {
            var result = new ApiResponse<List<GetEmployeeLeaveResponse>>();

            if (adminId == 0)
            {
                result.Messages.Add("AdminId not found for the given admin.");
                result.Success = false;
                result.Data = null;
            }

            var leaveRequests = await _adminRepository.GetEmployeeLeaveRequestsAsync(adminId);
            if (leaveRequests == null || !leaveRequests.Any())
            {
                result.Messages.Add("No leave requests found for the given admin.");
                result.Success = false;
                result.Data = null;
                return result;
            }
            result.Success = true;
            result.Data = leaveRequests;
            result.Messages.Add("Leave requests fetched successfully.");
            return result;
        }

        public async Task<ApiResponse<object>> UpdateLeaveStatus(UpdateLeaveStatusRequest leaveStatus)
        {
            var result = new ApiResponse<object>();
            var balancesToInsert = new List<LeaveBalanceResponse>();
            var balancesToUpdate = new List<LeaveBalanceResponse>();
            var responseMessages = new List<string>();
            var checkAdminId = await _adminRepository.CheckIfExisting(Id: leaveStatus.ReviewedBy);
            if (checkAdminId == false)
            {
                result.Messages.Add("User logged in cannot be found for updating leave status");
                result.Data = null;
                result.Success = false;
                return result;
            }
            var leave = await _adminRepository.UpdateLeaveStatus(leaveStatus);
            if (leave.RowsAffected == 0)
            {
                result.Success = false;
                result.Messages.Add("Failed to update leave status");
                result.Data = null;
                return result;
            }
            var fetchEmpDetailsForMail = await _employeeRepository.GetEmailByEmployeeId(leave.UpdatedLeave.EmployeeId);
            if (leave.UpdatedLeave?.Status == "Approved")
            {
                var sendApprovedEmail = await _emailService.SendLeaveApprovalMail(fetchEmpDetailsForMail);
                //fetch holidays
                var holidays = await _adminRepository.GetCompanyHolidays(leave.UpdatedLeave.CompanyId);
                //fetch leavetype this employee applied for
                var leaveTypeFetch = await _adminRepository.GetLeaveTypeAsync(leave.UpdatedLeave.LeaveTypeId);
                //fetch leave balance for this employee
                var fetchLeaveBalance = await _leaveRepository.GetLeaveBalancesAsync(leave.UpdatedLeave.EmployeeId, leave.UpdatedLeave.LeaveTypeId);
                // Count holidays inside the applied leave range
                int holidayCount = holidays
                    .Count(hol => hol.HolidayDate >= leave.UpdatedLeave.StartDate
                               && hol.HolidayDate <= leave.UpdatedLeave.EndDate);

                // Effective leave days = Applied days - Holidays
                int effectiveLeaveDays = leave.UpdatedLeave.TotalDays - holidayCount;
                if (effectiveLeaveDays < 0) effectiveLeaveDays = 0; // just in case
                if (fetchLeaveBalance == null || !fetchLeaveBalance.Any())
                {
                    var add = new LeaveBalanceResponse
                    {
                        LeaveTypeId = leave.UpdatedLeave.LeaveTypeId,
                        EmployeeId = leave.UpdatedLeave.EmployeeId,
                        TotalAllocatedDays = leaveTypeFetch.DefaultAllocation,
                        TotalTakenDays = effectiveLeaveDays,
                        RemainingDays = leaveTypeFetch.DefaultAllocation - effectiveLeaveDays,
                        Year = DateTime.UtcNow.Year,
                        CompanyId = leave.UpdatedLeave.CompanyId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false
                    };
                    balancesToInsert.Add(add);
                }
                else if (fetchLeaveBalance != null || fetchLeaveBalance.Any())
                {
                    // Ideally only 1 record should exist per Employee + LeaveType + Year
                    var balance = fetchLeaveBalance.First();
                    if (leave.UpdatedLeave.Status.Equals(LeaveStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        int newTotalTaken = balance.TotalTakenDays + effectiveLeaveDays;
                        if (newTotalTaken > balance.TotalAllocatedDays)
                        {
                            responseMessages.Add("Leave request exceeds allocated days.");
                        }
                        balance.TotalTakenDays = newTotalTaken;
                        balance.RemainingDays = balance.TotalAllocatedDays - newTotalTaken;
                        balance.UpdatedAt = DateTime.UtcNow;
                        balancesToUpdate.Add(balance);
                    }
                }
                if (balancesToInsert.Any())
                {
                    var addLeaveBalance = await _leaveRepository.AddLeaveBalanceAsync(balancesToInsert);
                    if (addLeaveBalance == false)
                    {
                        responseMessages.Add("Failed to update leave balance after approval");
                    }
                }
                if (balancesToUpdate.Any())
                {
                    var updateLeaveBalance = await _leaveRepository.UpdateLeaveBalanceAsync(balancesToUpdate);
                    if (updateLeaveBalance == false)
                    {
                        responseMessages.Add("Failed to update leave balance after approval");
                    }
                }
            }
            responseMessages.Add("Leave status updated successfully");
            result.Success = true;
            result.Messages.AddRange(responseMessages);
            result.Data = null;
            return result;
        }

        public async Task<ApiResponse<object>> GetFilteredLeaveRequestsAsync(LeaveFilterRequest filter)
        {
            var result = new ApiResponse<object>();

            if (filter.StartDate > filter.EndDate)
            {
                result.Success = false;
                result.Messages.Add("Start date cannot be after end date.");
                return result;
            }

            var leaveRequests = await _adminRepository.GetFilteredLeaveRequestsAsync(filter);
            if (leaveRequests == null || leaveRequests.Count == 0)
            {
                result.Success = false;
                result.Messages.Add("No leave requests can be found");
                return result;
            }
            result.Success = true;
            result.Messages.Add("Leave requests fetched successfully.");
            result.Data = leaveRequests;

            return result;
        }
    }
}
