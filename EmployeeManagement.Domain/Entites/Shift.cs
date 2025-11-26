using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entites
{
    public class Shift
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;   
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int BreakTime { get; set; }
        public bool isOvernight { get; set; } = false;
        public bool IsLate { get; set; }
        public double LateMinutes { get; set; }
        public bool EarlyExit { get; set; }
        public double OvertimeHours { get; set; }
        public bool IsAbsent { get; set; }
    }
}
