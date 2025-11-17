using LMS.DTOs.Admin;
using LMS.Model.Company;

namespace LMS.Interface
{
    public interface IHolidayService
    {
        Task<(bool Success, string Message)> UploadHolidayFileAsync(IFormFile file, int companyId);
        Task<List<GetHolidaysResponse>> GetHolidaysAsync(int companyId);
    }
}
