using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class PayrollSummaryDto
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public double TotalBasicSalary { get; set; }
        public double TotalOvertimePay { get; set; }
        public double TotalDeductions { get; set; }
        public double TotalNetPay { get; set; }
    }
}
