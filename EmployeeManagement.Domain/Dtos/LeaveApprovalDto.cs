using EmployeeManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class LeaveApprovalDto
    {
        public int LeaveId { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public string? Remmarks { get; set; }
    }
}
