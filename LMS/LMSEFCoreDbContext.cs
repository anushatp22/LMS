using LMS.Model.Admin;
using LMS.Model.Common;
using LMS.Model.Company;
using LMS.Model.Leave;
using Microsoft.EntityFrameworkCore;
using System;

namespace LMS
{
    public class LMSEFCoreDbContext : DbContext
    {
        public LMSEFCoreDbContext(DbContextOptions<LMSEFCoreDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Type = "Casual Leave", GenderRestriction = null, DefaultAllocation = 7, AllocationFrequency = "Yearly", IsActive = true, IsDeleted = false},
                new LeaveType { Id = 2, Type = "Sick Leave", GenderRestriction = null, DefaultAllocation = 12, AllocationFrequency = "Monthly", IsActive = true, IsDeleted = false },
                new LeaveType { Id = 3, Type = "Annual Leave", GenderRestriction = null, DefaultAllocation = 24, IsDeleted = false, IsActive = true, AllocationFrequency = "Yearly" },
                new LeaveType { Id = 4, Type = "Compensatory Leave", GenderRestriction = null, DefaultAllocation = 5, AllocationFrequency = "Yearly", IsActive = true, IsDeleted = false },
                new LeaveType { Id = 5, Type = "Earned Leave", GenderRestriction = null, DefaultAllocation = 15, AllocationFrequency = "Yearly", IsActive = true, IsDeleted = false },
                new LeaveType { Id= 6, Type = "Leave Without Pay", GenderRestriction = null, DefaultAllocation = 0, AllocationFrequency = "Yearly", IsActive = true, IsDeleted = false },
                new LeaveType { Id = 7, Type="Leave for miscarriage", GenderRestriction = "Female", DefaultAllocation = 42, IsDeleted = false, IsActive = true, AllocationFrequency = "Yearly" },
                new LeaveType { Id = 8, Type = "Maternity Leave", GenderRestriction = "Female", DefaultAllocation = 182, IsDeleted = false, IsActive = true, AllocationFrequency = "Yearly" },
                new LeaveType { Id = 9, Type = "Period Leave", GenderRestriction = "Female", DefaultAllocation = 12, IsDeleted = false, IsActive = true, AllocationFrequency = "Monthly" },
                new LeaveType { Id = 10, Type = "Paternity Leave", GenderRestriction = "Male", DefaultAllocation = 20, IsDeleted = false, IsActive = true, AllocationFrequency = "Yearly" },
                new LeaveType { Id = 11, Type = "Marriage Leave", GenderRestriction = null, DefaultAllocation = 10, IsDeleted = false, IsActive = true, AllocationFrequency = "Yearly" },
                new LeaveType { Id = 12, Type = "Bereavement Leave", GenderRestriction = null, DefaultAllocation = 5, IsDeleted = false, IsActive = true, AllocationFrequency = "Yearly" }
                );
        }
        public DbSet<EmployeeRegistration> employeeRegistration { get; set; }
        public DbSet<Company> company { get; set; }
        public DbSet<CompanyAdmin> companyAdmin { get; set; }
        public DbSet<LeaveType> leaveType { get; set; }
        public DbSet<LeaveBalance> leaveBalance { get; set; }

        public DbSet<CompanyHoliday> companyHoliday { get; set; }
        public DbSet<LeaveApplication> leaveApplication { get; set; }
        public DbSet<Notifications> notifications { get; set; }
    }
}
