namespace LMS.DTOs.Admin
{
    public class GetHolidaysResponse
    {
            public int Id { get; set; }
            public DateTime HolidayDate { get; set; }
            public string HolidayName { get; set; }
            public string HolidayType { get; set; }
            public int Year { get; set; }
            //public int CompanyId { get; set; }
        

    }
}
