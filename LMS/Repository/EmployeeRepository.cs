using LMS.Controllers;
using LMS.DTOs.Admin;
using LMS.DTOs.Common;
using LMS.DTOs.Employee;
using LMS.Migrations;
using LMS.Model.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMS.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LMSEFCoreDbContext _context;
        public EmployeeRepository(LMSEFCoreDbContext context)
        {
            _context = context;
        }

        public async Task<LoginConfirmJWTResponse?> ConfirmUser(string email, string password)
        {
            var result = await _context.employeeRegistration
                .Where(e => e.Email == email && e.IsActive && !e.IsDeleted).SingleOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            var hasher = new PasswordHasher<Model.Admin.EmployeeRegistration>();
            var passwordVerificationResult = hasher.VerifyHashedPassword(result, result.PasswordHash, password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return null;
            }
            return new LoginConfirmJWTResponse
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.Email,
                EmployeeId = result.EmployeeId,
                Department = result.Department,
                Role = result.Role,
                ForcePasswordChange = result.ForcePasswordChange,
                CompanyId = result.CompanyId
            };

        }

        public async Task<bool> ResetPassword(int employeeId, string oldPassword, string newPassword)
        {
            var fetch = await _context.employeeRegistration.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (fetch == null)
            {
                return false;
            }
            fetch.PasswordHash = newPassword;
            fetch.ForcePasswordChange = false;
            _context.employeeRegistration.Update(fetch);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int?> GetCompanyIdByEmployeeId(int employeeId)
        {
            try
            {
                var companyId = await _context.employeeRegistration.Where(e => e.Id == employeeId).
                    Select(employeeId => employeeId.CompanyId).FirstOrDefaultAsync();
                return companyId;
            }
            catch
            {
                // Optionally log the exception here
                return null; // or throw an exception based on your error handling strategy
            }
        }
        public async Task<string?> GetEmailByEmployeeId(int employeeId)
        {
            try
            {
                var companyId = await _context.employeeRegistration.Where(e => e.Id == employeeId).
                    Select(employeeId => employeeId.Email).FirstOrDefaultAsync();
                return companyId;
            }
            catch
            {
                // Optionally log the exception here
                return null; // or throw an exception based on your error handling strategy
            }
        }

        public async Task<string?> GetLeaveRequest(int LeaveApplicationId)
        {
            var leave = await _context.leaveApplication
        .FirstOrDefaultAsync(la => la.Id == LeaveApplicationId);

            if (leave == null)
                return null;

            return leave.Status; 
        }

        public async Task<bool> CancelLeaveRequest(int  CancelApplicationId)
        {
            var checkLeaveRequest = await _context.leaveApplication.FindAsync(CancelApplicationId);
            if (checkLeaveRequest == null)
            {
                return false;
            }
            checkLeaveRequest.Status = "Cancelled";
            checkLeaveRequest.IsActive = false;
            checkLeaveRequest.IsDeleted = true;
            checkLeaveRequest.UpdatedAt = DateTime.UtcNow;

            var updatedRows = await _context.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<List<GetLeaveRequestResponse>> GetLeaveRequestsForEmp(int userId, int? Id)
        {
            var query = _context.leaveApplication
                        .Where(l => l.IsActive && !l.IsDeleted && l.EmployeeId == userId)
                        .AsQueryable();

            if (Id.HasValue)
            {
                query = query.Where(l => l.Id == Id.Value); //optional id filter
            }

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
                          LeaveType = lt.Type,
                          StartDate = temp.la.StartDate,
                          EndDate = temp.la.EndDate,
                          Status = temp.la.Status,
                          TotalDays = temp.la.TotalDays,
                          Reason = temp.la.Reason,
                          AppliedAt = temp.la.AppliedAt,
                          DateApproved = temp.la.DateApproved
                      })
                .ToListAsync();
            if (result.Count == 0)
            {
                return null;
            }
            return result;
        }

        public async Task<List<GetLeaveBalanceForEmployeeResponse>> GetLeaveBalance(int UserId)
        {
            var lb = _context.leaveBalance.Where(l => l.EmployeeId == UserId && l.Year == DateTime.UtcNow.Year && !l.IsDeleted && l.IsActive).AsQueryable();
            var result = await lb
                .Join(_context.leaveType,
             lb => lb.LeaveTypeId,
             lt => lt.Id,
             (lb, lt) => new GetLeaveBalanceForEmployeeResponse
             {
                 LeaveTypeId = lb.LeaveTypeId,
                 LeaveType = lt.Type, // fetch name from leaveType table
                 TotalTakenDays = lb.TotalTakenDays,
                 RemainingDays = lb.RemainingDays
             })
            .ToListAsync();
            if (result.Count == 0)
            {
                return null;
            }
            return result;

        }
        public async Task<LoginConfirmJWTResponse?> GetUserById(string userId)
        {
            return await _context.employeeRegistration
                .Where(e => e.Id.ToString() == userId)
                .Select(e => new LoginConfirmJWTResponse
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Role = e.Role,
                    Department = e.Department
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Notifications>> GetNotificationsByUserAsync(int userId)
        {
            return await _context.notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AddNotificationAsync(NotificationResponse notification)
        {
            // Map DTO → Entity
            var noti = new Notifications
            {
                Id = Guid.NewGuid(),
                UserId = notification.UserId,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = DateTime.UtcNow
            };
            _context.notifications.Add(noti);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;

            }
            return false;
        }

        public async Task<Notifications> GetByIdAsync(Guid id)
        {
            return await _context.notifications.FindAsync(id);
        }

        public async Task UpdateAsync(Notifications notification)
        {
            _context.notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetGenderById(int userID)
        {
            return await _context.employeeRegistration.Where(a => a.Id == userID).Select(a => a.Gender).FirstOrDefaultAsync();
        }
    }
}
