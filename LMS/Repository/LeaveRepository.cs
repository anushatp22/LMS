using LMS.DTOs.Employee;
using LMS.Model.Leave;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;

namespace LMS.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly LMSEFCoreDbContext _context;
        public LeaveRepository(LMSEFCoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveResponse>> GetAllActiveEmployeesAsync()
        {
            var employees = await _context.employeeRegistration
                .Where(e => e.IsActive && !e.IsDeleted).
                Select(emp => new LeaveResponse
                {
                    UserId = emp.Id,
                    Name = emp.Name,
                    EmployeeId = emp.EmployeeId,
                    CompanyId = emp.CompanyId,
                    Gender = emp.Gender,
                    IsActive = emp.IsActive,
                    IsDeleted = emp.IsDeleted
                })
                .ToListAsync();
            return employees;
        }

        public async Task<List<LeaveTypes>> GetLeaveTypesAsync(List<string> requestValue)
        {
            var leaveType = await _context.leaveType
                .Where(lt => lt.IsActive && !lt.IsDeleted && lt.AllocationFrequency != null && requestValue.Contains(lt.AllocationFrequency))
                .Select(leaveType => new LeaveTypes
                {
                    LeaveTypeId = leaveType.Id,
                    LeaveType = leaveType.Type,
                    GenderRestriction = leaveType.GenderRestriction,
                    DefaultAllocation = leaveType.DefaultAllocation,
                    AllocationFrequency = leaveType.AllocationFrequency,
                    IsActive = leaveType.IsActive,
                    IsDeleted = leaveType.IsDeleted
                }).ToListAsync();
            return leaveType;
        }

        public async Task<List<LeaveBalanceResponse>> GetLeaveBalancesAsync(int? employeeId = null,
    int? leaveTypeId = null, int? year = null)
        {
            int targetYear = year ?? DateTime.Now.Year;
            var leaveBalance = await _context.leaveBalance
                .Where(lb => !lb.IsDeleted && lb.IsActive
                && (employeeId == null || lb.EmployeeId == employeeId.Value)
                && (leaveTypeId == null || lb.LeaveTypeId == leaveTypeId.Value)
                && lb.Year == targetYear)
                .Select(balance => new LeaveBalanceResponse
                {
                    LeaveBalanceId = balance.Id,
                    LeaveTypeId = balance.LeaveTypeId,
                    EmployeeId = balance.EmployeeId,
                    TotalAllocatedDays = balance.TotalAllocatedDays,
                    TotalTakenDays = balance.TotalTakenDays,
                    RemainingDays = balance.RemainingDays,
                    Year = balance.Year,
                    CompanyId = balance.CompanyId,
                    IsActive = balance.IsActive,
                    IsDelete = balance.IsDeleted
                }).ToListAsync();
            return leaveBalance;
        }

        public async Task<bool> UpdateLeaveBalanceAsync(IEnumerable<LeaveBalanceResponse> leaveBalance)
        {
            //update incoming list to dict
            var updateDict = leaveBalance.ToDictionary(lb => lb.LeaveBalanceId);
            //get only records that needs to be updating
            var Ids = updateDict.Keys.ToList();
            var entities = await _context.leaveBalance.Where(e => Ids.Contains(e.Id)).ToListAsync();
            foreach (var ent in entities)
            {
                if (updateDict.TryGetValue(ent.Id, out var updateData))
                {
                    ent.TotalTakenDays = updateData.TotalTakenDays;
                    ent.RemainingDays = updateData.RemainingDays;
                    ent.UpdatedAt = updateData.UpdatedAt;
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result == 1)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddLeaveBalanceAsync(IEnumerable<LeaveBalanceResponse> leaveBalance)
        {
            var entity = leaveBalance.Select(lb => new LeaveBalance
            {
                LeaveTypeId = lb.LeaveTypeId,
                EmployeeId = lb.EmployeeId,
                TotalAllocatedDays = lb.TotalAllocatedDays,
                TotalTakenDays = lb.TotalTakenDays,
                RemainingDays = lb.RemainingDays,
                Year = lb.Year,
                CompanyId = lb.CompanyId,
                CreatedAt = lb.CreatedAt,
                IsActive = lb.IsActive,
                IsDeleted = lb.IsDelete
            });
            await _context.leaveBalance.AddRangeAsync(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddLeaveRequest(LeaveApplication requestValue)
        {
            
            await _context.leaveApplication.AddAsync(requestValue);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<GetLeaveTypes>> GetLeaveTypesAsync(string Gender)
        {
            var data = await _context.leaveType
                .Where(a => a.GenderRestriction == null || a.GenderRestriction == Gender)
                .Select(a => new GetLeaveTypes
                    {
                       LeaveTypeId = a.Id,
                       LeaveTypeName = a.Type
                    })
                    .ToListAsync();
            if (data == null)
            {
                return null;
            }
            return data;
        }
    }
}
