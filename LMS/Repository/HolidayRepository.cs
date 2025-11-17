using LMS.Controllers;
using LMS.Model.Company;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly LMSEFCoreDbContext _context;
        public HolidayRepository(LMSEFCoreDbContext context)
        {
            _context = context;
        }
        public async Task BulkInsertHolidaysAsync(List<CompanyHoliday> holidays)
        {
            await _context.companyHoliday.AddRangeAsync(holidays);
            await _context.SaveChangesAsync();
        }
        public async Task<List<CompanyHoliday>> GetHolidaysByCompanyIdAsync(int companyId)
        {
            return await _context.companyHoliday
                .Where(h => h.CompanyId == companyId)
                .OrderBy(h => h.HolidayDate)
                .ToListAsync();
        }
    }
}
