using LMS.DTOs.Common;
using LMS.DTOs.Employee;
using LMS.Interface;
using LMS.Model.Common;
using LMS.Repository;
using LMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Employee : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJWTService _jwtService;
        private readonly IEmployeeRepository _employeeRepository;

        public Employee(IEmployeeService employeeService, IJWTService jwtService, IEmployeeRepository employeeRepository)
        {
            _employeeService = employeeService;
            _jwtService = jwtService;
            _employeeRepository = employeeRepository;
        }

        [HttpPost]
        [Route("User/Login")]
        public async Task<IActionResult> LoginUser([FromBody] DTOs.Employee.LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest("Invalid login request.");
            }
            try
            {
                var result = await _employeeService.LoginUser(loginRequest);
                if (result.Success || result.Data.ForcePasswordChange != null)
                {
                    // Set HttpOnly refresh token cookie
                    //Response.Cookies.Append("refreshToken", result.Data.RefreshToken, new CookieOptions
                    //{
                    //    HttpOnly = true,
                    //    Secure = true,
                    //    //SameSite = SameSiteMode.Strict,
                    //    SameSite = SameSiteMode.None,
                    //    Expires = result.Data.RefreshTokenExpiry
                    //});
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPatch]
        [Route("User/{id}/ResetPassword")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] PasswordResetRequest passwordReset)
        {
            try
            {
                var result = await _employeeService.PasswordResetRequest(id, passwordReset);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("User/CancelLeaveRequest/{id}")]
        public async Task<IActionResult> CancelLeaveRequest(int id)
        {
            try
            {
                var result = await _employeeService.CancelLeaveRequest(id);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet]
        [Route("GetLeaveRequests/{UserId}")]
        public async Task<IActionResult> GetLeaveRequests(int UserId, [FromQuery] int? Id = null)
        {
            try
            {
                var result = await _employeeService.GetLeaveRequest(UserId, Id);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("GetLeaveBalance/{UserId}")]
        public async Task<IActionResult> GetLeaveBakance(int UserId)
        {
            try
            {
                var result = await _employeeService.GetLeaveBalance(UserId);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetNotifications/{userId}")]
        public async Task<IActionResult> GetNotifications(int userId)
        {

            try
            {
                var result = await _employeeService.GetNotifications(userId);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("AddNotification")]
        public async Task<IActionResult> AddNotification([FromBody] NotificationResponse notification)
        {
            try
            {
                var response = await _employeeService.AddNotificationAsync(notification);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("MarkAsRead/{id}")]

        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var response = await _employeeService.MarkAsReadAsync(id);
                if (response == null)
                    return NotFound(new { message = "Notification not found" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }



    }
}
