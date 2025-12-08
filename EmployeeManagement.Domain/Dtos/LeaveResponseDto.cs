using EmployeeManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class LeaveResponseDto
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }

        public LeaveType LeaveType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }

        public Status Status { get; set; }
        public string? ManagerComments { get; set; }
    }
}
