using LMS.DTOs.Admin;
using LMS.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class Holiday : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public Holiday(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        [HttpGet]
        [Route("DownloadHolidayTemplate")]
        public IActionResult DownloadHolidayTemplate([FromServices] IWebHostEnvironment env)
        {
            var filePath = Path.Combine(env.WebRootPath, "Templates", "LMS_Holiday_Template.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Template not found.");

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "HolidayTemplate.xlsx");
        }

        [HttpPost("UploadHolidayFile")]
        [RequestSizeLimit(10_000_000)] // 10 MB limit
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadHolidayFile(GetHolidayUploadRequest uploadRequest)
        {
            if (uploadRequest.File == null || uploadRequest.File.Length == 0)
                return BadRequest("No file uploaded.");
            if (!int.TryParse(uploadRequest.CompanyId, out int companyIdNumber))
                return BadRequest("Invalid Company ID.");
            var isSuccess = await _holidayService.UploadHolidayFileAsync(uploadRequest.File, companyIdNumber);

            if (!isSuccess.Success)
                return BadRequest("Invalid file or upload failed.");

            return Ok(new { message = "File uploaded successfully" });
        }

        [HttpGet]
        [Route("Company/{companyId}")]
        public async Task<IActionResult> GetHolidaysByCompany(int companyId)
        {
            try
            {
                var holidays = await _holidayService.GetHolidaysAsync(companyId);
                return Ok(new { data = holidays });
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to fetch data");
            }
        }
    }
}
