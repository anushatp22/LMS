using System.ComponentModel.DataAnnotations;

namespace LMS.Model.Company
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
