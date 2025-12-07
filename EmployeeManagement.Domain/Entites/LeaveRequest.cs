using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Entites
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public int TotalDays { get; set; }
        public  LeaveType LeaveType { get; set; } =LeaveType.Annaual;
        public Status Status { get; set; } = Status.Pending;

        public string? ManagerComments { get; set; } 
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    }
}
