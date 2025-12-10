using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class LeaveStatsDto
    {
        public int TotalLeaveRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int AnnualLeaveRequest { get; set; }
        public int MaternityLeaveRequest { get; set; }
        public int PaternityLeaveRequest { get; set; }
        public int CasualLeaveRequest { get; set; }
    }
}
