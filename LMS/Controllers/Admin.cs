using LMS.DTOs.Admin;
using LMS.Interface;
using LMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Admin : ControllerBase
    {
        private readonly IAdminService _adminService;

        public Admin(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize]
        [HttpPost]
        [Route("RegisterEmployee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployee registerEmployee)
        {
            if (registerEmployee == null)
            {
                Log.Warning("Received null employee data in RegisterEmployee request.");
                return BadRequest("Invalid employee data.");
            }
            Log.Information("Attempting to register new employee with EmployeeId: {EmployeeId}, Name: {Name}, CompanyId: {CompanyId}",
            registerEmployee.EmployeeId,
            registerEmployee.Name,
            registerEmployee.CompanyId);
            var result = await _adminService.AddEmployee(registerEmployee);
            if (result.Success)
            {
                Log.Information("Successfully registered employee. EmployeeId: {EmployeeId}, CompanyId: {CompanyId}",
                registerEmployee.EmployeeId,
                registerEmployee.CompanyId);
                return Ok(result);
            }
            else
            {

                return StatusCode(500, result);
            }
        }

        [HttpGet]
        [Route("Admin/ViewEmployeeLeaveRequest/{id}")]
        public async Task<IActionResult> GetEmployeeLeaveRequests(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid UserId.");

            var result = await _adminService.GetEmployeeLeaveRequestsAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }

        [HttpPost]
        [Route("UpdateLeaveStatus")]
        public async Task<IActionResult> UpdateLeaveStatus([FromBody] UpdateLeaveStatusRequest request)
        {
            if (request == null)
                return BadRequest("Invalid Request");
            var result = await _adminService.UpdateLeaveStatus(
                request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("Filter")]
        public async Task<IActionResult> FilterLeaveRequests([FromBody] LeaveFilterRequest filter)
        {
            if ((!filter.StartDate.HasValue && !filter.EndDate.HasValue) && filter.Status == null)
            {
                return BadRequest("Please apply filter");
            }
            var response = await _adminService.GetFilteredLeaveRequestsAsync(filter);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
