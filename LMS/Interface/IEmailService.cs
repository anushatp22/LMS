namespace LMS.Interface
{
    public interface IEmailService
    {
        Task<bool> SendTempPasswordAsync(string email, string password, string companyName);
        Task<bool> SendLeaveApprovalMail(string Email);
    }
}
