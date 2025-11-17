namespace LMS.DTOs.Admin
{
    public class GetHolidayUploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public string CompanyId { get; set; }
    }
}
