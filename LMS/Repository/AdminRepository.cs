using LMS.DTOs.Admin;
using LMS.DTOs.Employee;
using LMS.Model.Company;
using LMS.Model.Leave;
using LMS.ResponseModel;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository
{
    public class AdminRepository : IAdminRepository
    {

        private readonly LMSEFCoreDbContext _context;
        public AdminRepository(LMSEFCoreDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeRegisteredLeaveAllocation?> AddEmployee(RegisterEmployee registerEmployee)
        {
            try
            {
                var entity = new Model.Admin.EmployeeRegistration
                {
                    Name = registerEmployee.Name,
                    Email = registerEmployee.Email,
                    EmployeeId = registerEmployee.EmployeeId,
                    Department = registerEmployee.Department,
                    Role = registerEmployee.Role,
                    JoiningDate = registerEmployee.JoiningDate,
                    PasswordHash = registerEmployee.PasswordHash,
                    CompanyId = registerEmployee.CompanyId,
                    ForcePasswordChange = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = registerEmployee.Gender,
                    IsActive = true,
                    IsDeleted = false,
                };
                _context.employeeRegistration.Add(entity);
                await _context.SaveChangesAsync();
                return new EmployeeRegisteredLeaveAllocation
                {
                    Success = true,
                    EmployeeId = entity.Id,
                    Gender = entity.Gender,
                    CompanyId = entity.CompanyId
                };
            }
            catch (Exception)
            {
                // Optionally log the exception here
                return null;
            }
        }

        public async Task<CompanyInfo?> GetCompanyIdByAdminId(int adminId)
        {
            try
            {
                var companyDetails = await (from company in _context.company
                                            join companyAdmin in _context.companyAdmin
                                            on company.Id equals companyAdmin.CompanyId
                                            where companyAdmin.Id == adminId
                                            select new CompanyInfo
                                            {
                                                CompanyName = company.Name,
                                                CompanyId = company.Id
                                            }).FirstOrDefaultAsync();
                return companyDetails;
            }
            catch
            {
                // Optionally log the exception here
                return null; // or throw an exception based on your error handling strategy
            }
        }

        public async Task<bool> CheckIfExisting(string? email = null, int? Id = null)
        {
            if (Id == null && email == null)
                throw new ArgumentException("Either id or email must be provided.");
            var Check = await _context.employeeRegistration.AnyAsync(e => (Id != null && e.Id == Id) || (e.Email != null && e.Email == email));
            if (Check == false)
            {
                return false;
            }
            return true;
        }

        public async Task<List<GetEmployeeLeaveResponse>> GetEmployeeLeaveRequestsAsync(int adminId)
        {
            var fromDate = DateTime.UtcNow.AddDays(-30);
            // Get companyId from adminId
            var adminDetail = await _context.employeeRegistration
                    .Where(e => e.Id == adminId)
                    .Select(e => new
                    {
                        e.CompanyId,
                        e.Name
                    })
                    .FirstOrDefaultAsync();
            if (adminDetail?.CompanyId == 0)
                return new List<GetEmployeeLeaveResponse>();
            var leaveRequests = await (
                from lr in _context.leaveApplication
                join e in _context.employeeRegistration
                    on lr.EmployeeId equals e.Id
                join lt in _context.leaveType
                    on lr.LeaveTypeId equals lt.Id
                where lr.CompanyId == adminDetail.CompanyId && lr.IsDeleted == false && lr.CreatedAt >= fromDate && lr.IsActive == true
                select new GetEmployeeLeaveResponse
                {
                    LeaveRequestId = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = e.Name,
                    LeaveType = lt.Type,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    AppliedAt = lr.AppliedAt,
                    DateApproved = lr.DateApproved,
                    ReviewedBy = adminDetail.Name
                }).ToListAsync();
            return leaveRequests;
        }

        public async Task<UpdateLeaveStatusResult> UpdateLeaveStatus(UpdateLeaveStatusRequest leaveStatus)
        {
            var checkLeaveRequest = await _context.leaveApplication.FindAsync(leaveStatus.LeaveId);
            if (checkLeaveRequest == null)
                return new UpdateLeaveStatusResult
                {
                    RowsAffected = 0,
                    UpdatedLeave = null
                };
            checkLeaveRequest.Status = leaveStatus.NewStatus.ToString();
            checkLeaveRequest.ReviewedBy = leaveStatus.ReviewedBy;
            checkLeaveRequest.DateApproved = DateTime.UtcNow.Date;
            checkLeaveRequest.UpdatedAt = DateTime.UtcNow;

            var updatedRows = await _context.SaveChangesAsync();
            return new UpdateLeaveStatusResult
            {
                RowsAffected = updatedRows,
                UpdatedLeave = checkLeaveRequest
            };
        }

        public async Task<List<CompanyHoliday>> GetCompanyHolidays(int companyId)
        {
            var holidays = await _context.companyHoliday
                .Where(ch => ch.CompanyId == companyId && ch.IsActive == true && ch.IsDeleted == false && ch.Year == DateTime.UtcNow.Year)
                .ToListAsync();
            return holidays;
        }
        public async Task<LeaveTypes?> GetLeaveTypeAsync(int LeaveTypeId)
        {
            var leaveType = await _context.leaveType
                .Where(lt => lt.IsActive && !lt.IsDeleted && lt.AllocationFrequency != null && lt.Id == LeaveTypeId)
                .Select(leaveType => new LeaveTypes
                {
                    LeaveTypeId = leaveType.Id,
                    LeaveType = leaveType.Type,
                    GenderRestriction = leaveType.GenderRestriction,
                    DefaultAllocation = leaveType.DefaultAllocation,
                    AllocationFrequency = leaveType.AllocationFrequency,
                    IsActive = leaveType.IsActive,
                    IsDeleted = leaveType.IsDeleted
                }).FirstOrDefaultAsync();
            return leaveType;
        }

        public async Task<List<GetLeaveRequestResponse>> GetFilteredLeaveRequestsAsync(LeaveFilterRequest filter)
        {
            // dynamic queries -- .AsQueryable()
            //→ This ensures the result stays as an IQueryable instead of executing the query immediately.
            //It means you can add more filters later dynamically(like status, date, etc.) before hitting the database.
            //The final SQL query is only sent to the database when you call something like .ToListAsync()
            var query = _context.leaveApplication
                .Where(la => !la.IsDeleted && la.IsActive) // common filter
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                query = query.Where(la => la.Status == filter.Status.Value.ToString());
            }

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var start = filter.StartDate.Value.Date; // 00:00:00
                var end = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1); // 23:59:59.9999999
                query = query.Where(la => la.StartDate >= start && la.EndDate <= end);
            }
            // join with employeeRegister + leaveType
            var result = await query
                .Join(_context.employeeRegistration,
                      la => la.EmployeeId,
                      er => er.Id,
                      (la, er) => new { la, er })
                .Join(_context.leaveType,
                      temp => temp.la.LeaveTypeId,
                      lt => lt.Id,
                      (temp, lt) => new GetLeaveRequestResponse
                      {
                          LeaveRequestId = temp.la.Id,
                          EmployeeId = temp.la.EmployeeId,
                          EmployeeName = temp.er.Name,
                          LeaveTypeId = temp.la.LeaveTypeId,
                          LeaveType = lt.Type,   // assuming column is TypeName
                          StartDate = temp.la.StartDate,
                          EndDate = temp.la.EndDate,
                          Status = temp.la.Status,
                          TotalDays = temp.la.TotalDays,
                          Reason = temp.la.Reason,
                          AppliedAt = temp.la.AppliedAt,
                          DateApproved = temp.la.DateApproved
                      })
                .ToListAsync();

            return result;
        }
    }
}
