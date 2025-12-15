
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Persistence
{
    public class EmployeeManagementDbContext: IdentityDbContext<AppUser>
    {
        public EmployeeManagementDbContext(DbContextOptions<EmployeeManagementDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Employee>()
                .Property(e => e.BasicSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
                 .HasOne(e => e.Shift)
                 .WithMany()
                 .HasForeignKey(e => e.ShiftId)
                 .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
    }
}
