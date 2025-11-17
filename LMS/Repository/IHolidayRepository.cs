using LMS.Model.Company;

namespace LMS.Repository
{
    public interface IHolidayRepository
    {
        Task BulkInsertHolidaysAsync(List<CompanyHoliday> holidays);
        Task<List<CompanyHoliday>> GetHolidaysByCompanyIdAsync(int companyId);
    }
}
