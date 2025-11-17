using LMS.DTOs.Admin;
using LMS.DTOs.Employee;
using LMS.Interface;
using LMS.Service;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Leave : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public Leave(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpPost]
        [Route("AddLeaveApplication")]
        public async Task<IActionResult> AddLeaveRequest([FromBody]LeaveRequest leaveRequest)
        {
            if (leaveRequest == null)
            {
                //Log.Warning("Received null employee data in RegisterEmployee request.");
                return BadRequest("Invalid employee data.");
            }
            var result = await _leaveService.AddLeaveRequest(leaveRequest);
            if (result == true)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        [Route("LeaveTypes/{userID}")]
        public async Task<IActionResult> GetLeaveTypes(int userID)
        {
            var result = await _leaveService.GetLeaveTypes(userID);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
