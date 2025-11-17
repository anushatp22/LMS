using LMS.DTOs.Employee;
using LMS.Interface;
using LMS.Model.Leave;
using LMS.Repository;
using LMS.ResponseModel;

namespace LMS.Service
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveService(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task AllocateMonthlyLeavesAsync()
        {
            var employees = await _leaveRepository.GetAllActiveEmployeesAsync();
            var leaveTypes = await _leaveRepository.GetLeaveTypesAsync(new List<string> { "Monthly" });
            var leaveBalance = await _leaveRepository.GetLeaveBalancesAsync();
            var balancesToUpdate = new List<LeaveBalanceResponse>();
            var balancesToInsert = new List<LeaveBalanceResponse>();
            var leaveBalanceDict = leaveBalance.ToDictionary(lb => (lb.EmployeeId, lb.LeaveTypeId)); // tuple key
            foreach (var employee in employees)
            {
                foreach (var leaveType in leaveTypes)
                {
                    if (leaveType.GenderRestriction == null || leaveType.GenderRestriction == employee.Gender)
                    {
                        if (leaveBalanceDict.TryGetValue((employee.UserId, leaveType.LeaveTypeId), out var checkingEmployeeInLeaveBalance))
                        {
                            if (checkingEmployeeInLeaveBalance.RemainingDays < leaveType.DefaultAllocation)
                            {
                                checkingEmployeeInLeaveBalance.RemainingDays += 1;
                                checkingEmployeeInLeaveBalance.UpdatedAt = DateTime.UtcNow;
                                balancesToUpdate.Add(checkingEmployeeInLeaveBalance);
                            }
                        }
                        else
                        {
                            var add = new LeaveBalanceResponse
                            {
                                LeaveTypeId = leaveType.LeaveTypeId,
                                EmployeeId = employee.UserId,
                                TotalAllocatedDays = leaveType.DefaultAllocation,
                                TotalTakenDays = 0,
                                RemainingDays = 1,
                                Year = DateTime.UtcNow.Year,
                                CompanyId = employee.CompanyId,
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true,
                                IsDelete = false
                            };
                            balancesToInsert.Add(add);
                        }
                    }
                }
            }
            if (balancesToUpdate.Any())
                await _leaveRepository.UpdateLeaveBalanceAsync(balancesToUpdate);

            if (balancesToInsert.Any())
                await _leaveRepository.AddLeaveBalanceAsync(balancesToInsert);
        }

        public async Task AllocateAnnualLeaveAsync()
        {
            var employees = await _leaveRepository.GetAllActiveEmployeesAsync();
            var leaveTypes = await _leaveRepository.GetLeaveTypesAsync(new List<String> { "Yearly" });
            var leaveBalance = await _leaveRepository.GetLeaveBalancesAsync(DateTime.UtcNow.Year);
            var balancesToUpdate = new List<LeaveBalanceResponse>();
            var balancesToInsert = new List<LeaveBalanceResponse>();
            var leaveBalanceDict = leaveBalance.ToDictionary(lb => (lb.EmployeeId, lb.LeaveTypeId)); // tuple key
            foreach (var employee in employees)
            {
                foreach (var leaveType in leaveTypes)
                {
                    if (leaveType.GenderRestriction == null || leaveType.GenderRestriction == employee.Gender)
                    {
                        if (!leaveBalanceDict.ContainsKey((employee.UserId, leaveType.LeaveTypeId)))
                        {
                            var add = new LeaveBalanceResponse
                            {
                                LeaveTypeId = leaveType.LeaveTypeId,
                                EmployeeId = employee.UserId,
                                TotalAllocatedDays = leaveType.DefaultAllocation,
                                TotalTakenDays = 0,
                                RemainingDays = leaveType.DefaultAllocation,
                                Year = DateTime.UtcNow.Year,
                                CompanyId = employee.CompanyId,
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true,
                                IsDelete = false
                            };
                            balancesToInsert.Add(add);
                        }
                    }
                    else
                    {
                        //do logging
                    }
                }
            }
            if (balancesToUpdate.Any())
                await _leaveRepository.UpdateLeaveBalanceAsync(balancesToUpdate);

            if (balancesToInsert.Any())
                await _leaveRepository.AddLeaveBalanceAsync(balancesToInsert);
        }

        public async Task<bool> AllocateNewEmployeeLeave(EmployeeRegisteredLeaveAllocation employeeRegisteredLeaveAllocation)
        {
            var employeeLeaveTypes = await _leaveRepository.GetLeaveTypesAsync(new List<string> { "Yearly", "Monthly" });
            //var leaveBalance = await _leaveRepository.GetLeaveBalancesAsync();
            //var checkInLeaveBalance = leaveBalance.ToDictionary(lb => (lb.EmployeeId, lb.LeaveTypeId));
            var balancesToInsert = new List<LeaveBalanceResponse>();
            foreach (var leaveType in employeeLeaveTypes)
            {
                if (leaveType.GenderRestriction == null || leaveType.GenderRestriction == employeeRegisteredLeaveAllocation.Gender)
                {
                    //if (checkInLeaveBalance.ContainsKey((employeeRegisteredLeaveAllocation.EmployeeId, leaveType.LeaveTypeId)))
                    //{
                    //    throw new Exception($"Leave balance found for New EmployeeId: {employeeRegisteredLeaveAllocation.EmployeeId}, LeaveTypeId: {leaveType.LeaveTypeId}. Check if new Employee or EmployeeID already exists");
                    //}
                    var add = new LeaveBalanceResponse
                    {
                        LeaveTypeId = leaveType.LeaveTypeId,
                        EmployeeId = employeeRegisteredLeaveAllocation.EmployeeId,
                        TotalAllocatedDays = leaveType.DefaultAllocation,
                        TotalTakenDays = 0,
                        RemainingDays = leaveType.AllocationFrequency == "Yearly" ? leaveType.DefaultAllocation : 1,
                        Year = DateTime.UtcNow.Year,
                        CompanyId = employeeRegisteredLeaveAllocation.CompanyId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false
                    };
                    balancesToInsert.Add(add);
                }

            }
            if (balancesToInsert.Any())
            {
                var result = await _leaveRepository.AddLeaveBalanceAsync(balancesToInsert);
                return result;
            }
            return false;
        }

        public async Task<bool> AddLeaveRequest(DTOs.Employee.LeaveRequest leaveRequest)
        {
            if (leaveRequest.StartDate > leaveRequest.EndDate)
                throw new ArgumentException("Start date cannot be greater than end date");

            var totalDays = (leaveRequest.EndDate - leaveRequest.StartDate).Days + 1;
            leaveRequest.AppliedAt = DateTime.UtcNow;

            var getCompanyId = await _employeeRepository.GetCompanyIdByEmployeeId(leaveRequest.EmployeeId);
            if (getCompanyId == null)
                throw new ArgumentException("Invalid EmployeeId or CompanyId");
            var entity = new LeaveApplication
            {
                EmployeeId = leaveRequest.EmployeeId,
                LeaveTypeId = leaveRequest.LeaveTypeId,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                TotalDays = totalDays,
                Reason = leaveRequest.Reason,
                Status = "pending",
                AppliedAt = leaveRequest.AppliedAt,
                CompanyId = getCompanyId.Value,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
            };
            var addLeaveRequest = await _leaveRepository.AddLeaveRequest(entity);
            return addLeaveRequest;
        }
        public async Task<List<GetLeaveTypes>> GetLeaveTypes(int userID)
        {
            var gender = await _employeeRepository.GetGenderById(userID);
            if (gender != null)
            {
                return await _leaveRepository.GetLeaveTypesAsync(gender);

            }
            return null;
        }
    }
}
