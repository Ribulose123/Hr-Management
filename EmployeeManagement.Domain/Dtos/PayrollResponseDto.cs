using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class PayrollResponseDto
    {
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public double BasicSalary { get; set; }
        public double TotalHoursWorked { get; set; }
        public double OvertimeHours { get; set; }
        public double OvertimePay { get; set; }
        public double LateHours { get; set; }
        public int AbsenceDays { get; set; }
        public double AbsenceDeduction { get; set; }
        public double LatenessDeduction { get; set; }
        public double GrossPay { get; set; }
        public double NetPay { get; set; }
    }

}
