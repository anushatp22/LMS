using System.ComponentModel.DataAnnotations;

namespace LMS.ResponseModel
{
    public class EmployeeRegistrationResult
    {
        public bool IsRegistered { get; set; }
        public bool EmailSent { get; set; }
    }

    public class EmployeeRegisteredLeaveAllocation
    {
        public bool Success { get; set; }
        public int EmployeeId { get; set; }
        public string Gender { get; set; }
        public int CompanyId { get; set; }
    }
}
