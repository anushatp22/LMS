using Azure;
using LMS.DTOs.Admin;
using LMS.DTOs.Common;
using LMS.DTOs.Employee;
using LMS.Interface;
using LMS.Model.Common;
using LMS.Repository;
using LMS.ResponseModel.Common;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Json;

namespace LMS.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJWTService _jwtService;
        private readonly IAdminRepository _adminRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IJWTService jwtService, IAdminRepository adminRepository)
        {
            _employeeRepository = employeeRepository;
            _jwtService = jwtService;
            _adminRepository = adminRepository;
        }

        public async Task<JwtLoginResponse<LoginTokens>> LoginUser(LoginRequest loginRequest)
        {
            var response = new JwtLoginResponse<object>();
            var employeeExists = await _employeeRepository.ConfirmUser(loginRequest.Email, loginRequest.Password);
            if (employeeExists == null)
            {
                return new JwtLoginResponse<LoginTokens>
                {
                    Success = false,
                    Messages = new List<string> { "Invalid email or password." },
                    Data = null
                };
            }
            if (employeeExists.ForcePasswordChange == true)
            {
                return new JwtLoginResponse<LoginTokens>
                {
                    Success = false,
                    Messages = new List<string> { "Please update password." },
                    Data = new LoginTokens
                    {
                        Email = employeeExists.Email,
                        EmployeeId = employeeExists.Id,
                        ForcePasswordChange = employeeExists.ForcePasswordChange
                    }
                };
            }
            var token = await _jwtService.GenerateToken(employeeExists, loginRequest);
            //var refreshToken = _jwtService.GenerateRefreshToken();
            return new JwtLoginResponse<LoginTokens>
            {
                Success = true,
                Messages = new List<string> { "Login successful." },
                Data = new LoginTokens
                {
                    AccessToken = token,
                    //RefreshToken = refreshToken.Token,
                    //RefreshTokenExpiry = refreshToken.Expires
                }
            };
        }

        public async Task<ApiResponse<object>> PasswordResetRequest(int id, PasswordResetRequest passwordReset)
        {
            var response = new ApiResponse<object>();
            var hasher = new PasswordHasher<PasswordResetRequest>();
            passwordReset.NewPassword = hasher.HashPassword(passwordReset, passwordReset.NewPassword);
            var changePassword = await _employeeRepository.ResetPassword(id, passwordReset.OldPassword, passwordReset.NewPassword);
            if (changePassword == false)
            {
                response.Success = false;
                response.Messages.Add("Please try again.");
                return response;
            }
            response.Success = true;
            response.Messages.Add("Password updated successfully.");
            return response;
        }

        public async Task<ApiResponse<object>> CancelLeaveRequest(int Id)
        {
            var response = new ApiResponse<object>();
            if (Id == 0)
            {
                response.Messages.Add("Invalid Id");
                response.Success = false;
                response.Data = null;
                return response;
            }
          
            var isExisting = await _employeeRepository.GetLeaveRequest(Id);
            if (isExisting == null)
            {
                response.Messages.Add($"Couldnt find LeaveRequest with Id {Id}");
                response.Success = false;
                response.Data = null;
                return response;
            }
            if (isExisting != LeaveStatus.Pending.ToString())
            {
                response.Messages.Add("Users leave request status should be in pending status to cancel request");
                response.Success = false;
                response.Data = null;
                return response;
            }
            var updateCancelRequest = await _employeeRepository.CancelLeaveRequest(Id);
            if (updateCancelRequest == false)
            {
                response.Messages.Add("Failed to update cancel request");
                response.Success = false;
                response.Data = null;
                return response;
            }

            response.Messages.Add("Leave request is cancelled successfully");
            response.Success = true;
            response.Data = null;
            return response;

        }

        public async Task<ApiResponse<object>> GetLeaveRequest(int UserId, int? id)
        {
            var result = new ApiResponse<object>();
            var getRequests = await _employeeRepository.GetLeaveRequestsForEmp(UserId, id);
            if (getRequests != null)
            {
                result.Success = true;
                result.Data = getRequests;
                result.Messages.Add("Fetched data successfully");
                return result;
            }
            result.Success = false;
            result.Messages.Add("No data found");
            return result;
        }

        public async Task<ApiResponse<object>> GetLeaveBalance(int UserId)
        {
            var result = new ApiResponse<object>();
            var getLeaveBalance = await _employeeRepository.GetLeaveBalance(UserId);
            if (getLeaveBalance != null)
            {
                result.Success = true;
                result.Data = getLeaveBalance;
                result.Messages.Add("Fetched data successfully");
                return result;
            }
            result.Success = false;
            result.Messages.Add("No data found");
            return result;
        }

        public async Task<ApiResponse<object>> GetNotifications(int userId)
        {
            var result = new ApiResponse<object>();
            var getLeaveBalance = await _employeeRepository.GetNotificationsByUserAsync(userId);
            var arrangeNotification = getLeaveBalance.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
            });
            if (getLeaveBalance != null || !getLeaveBalance.Any())
            {
                result.Success = true;
                result.Data = getLeaveBalance;
                result.Messages.Add("No Notification found");
                return result;
            }
            result.Success = true;
            result.Messages.Add("Successfully fetched notifications");
            result.Data = arrangeNotification;
            return result;
        }

        public async Task<ApiResponse<object>> AddNotificationAsync(NotificationResponse notification)
        {
            var result = new ApiResponse<object>();
            var savedNotification = await _employeeRepository.AddNotificationAsync(notification);
            if (savedNotification == false)
            {
                result.Success = true;
                //result.Data = getLeaveBalance;
                result.Messages.Add("No Notification saved");
                return result;
            }
            result.Success = true;
            result.Messages.Add("Successfully saved notifications");
            //result.Data = arrangeNotification;
            return result;
        }

        public async Task<NotificationResponse?> MarkAsReadAsync(Guid id)
        {
            var notification = await _employeeRepository.GetByIdAsync(id);
            if (notification == null) return null;

            notification.IsRead = true;
            await _employeeRepository.UpdateAsync(notification);

            return new NotificationResponse
            {
                Id = notification.Id,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
