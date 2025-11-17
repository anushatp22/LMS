using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using LMS.Controllers;
using LMS.DTOs.Admin;
using LMS.Interface;
using LMS.Model.Company;
using LMS.Repository;
using System.Globalization;
using System.Text;

namespace LMS.Service
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _repository;
        private readonly IWebHostEnvironment _env;
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository repository, IWebHostEnvironment env, IHolidayRepository holidayRepository)
        {
            _repository = repository;
            _env = env;
            _holidayRepository = holidayRepository;
        }
        public async Task<(bool Success, string Message)> UploadHolidayFileAsync(IFormFile file, int companyId)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx" && ext != ".csv")
                return (false, "Only .xlsx or .csv files are allowed.");

            var uploadPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "Uploads", "HolidayFiles");
            Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, $"{Guid.NewGuid()}{ext}");
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                List<CompanyHoliday> holidays;

                if (ext == ".xlsx")
                    holidays = await ReadFromExcelAsync(filePath, companyId);
                else
                    holidays = await ReadFromCsvAsync(filePath, companyId);

                if (holidays.Count == 0)
                    return (false, "No valid records found in the file.");

                await _repository.BulkInsertHolidaysAsync(holidays);
                return (true, "Holiday file uploaded and saved successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error processing file: {ex.Message}");
            }
        }
        public async Task<List<GetHolidaysResponse>> GetHolidaysAsync(int companyId)
        {
            var holidays = await _holidayRepository.GetHolidaysByCompanyIdAsync(companyId);
            // Map to DTO
            var result = holidays.Select(h => new GetHolidaysResponse
            {
                Id = h.Id,
                HolidayDate = h.HolidayDate,
                HolidayName = h.HolidayName,
                HolidayType = h.HolidayType,
                Year = h.Year
            }).ToList();
            return result;
        }

        private async Task<List<CompanyHoliday>> ReadFromExcelAsync(string filePath, int companyId)
        {
            var list = new List<CompanyHoliday>();

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null) return list;

            // Assuming header row = 1
            int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var dataCell = worksheet.Cell(row, 1).GetString().Trim();
                    var nameCell = worksheet.Cell(row, 2).GetString().Trim();
                    var typeCell = worksheet.Cell(row, 3).GetString().Trim();
                    var yearCell = worksheet.Cell(row, 4).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(dataCell) || string.IsNullOrWhiteSpace(nameCell))
                        continue;

                    if (!DateTime.TryParse(dataCell, out var holidayDate))
                        continue;

                    int.TryParse(yearCell, out int year);
                    if (year == 0) year = holidayDate.Year;

                    list.Add(new CompanyHoliday
                    {
                        CompanyId = companyId, // Adjust based on login/company context
                        HolidayDate = holidayDate,
                        HolidayName = nameCell,
                        HolidayType = string.IsNullOrWhiteSpace(typeCell) ? "Public" : typeCell,
                        Year = year,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    });
                }
                catch
                {
                    // Skip invalid rows
                    continue;
                }
            }

            return await Task.FromResult(list);
        }

        private async Task<List<CompanyHoliday>> ReadFromCsvAsync(string filePath, int companyId)
        {
            var list = new List<CompanyHoliday>();

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            string? line;
            bool headerSkipped = false;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!headerSkipped)
                {
                    headerSkipped = true;
                    continue; // skip header
                }

                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                try
                {
                    var dateString = parts[0].Trim();
                    var name = parts[1].Trim();
                    var type = parts[2].Trim();
                    var yearText = parts[3].Trim();

                    if (!DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        continue;

                    int.TryParse(yearText, out int year);
                    if (year == 0) year = date.Year;

                    list.Add(new CompanyHoliday
                    {
                        CompanyId = companyId,
                        HolidayDate = date,
                        HolidayName = name,
                        HolidayType = type,
                        Year = year,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    });
                }
                catch
                {
                    continue;
                }
            }

            return list;
        }
    
    }
}
