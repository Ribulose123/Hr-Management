using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class PayrollMonthStatsDto
    {
        public int TotalEmployees { get; set; }
        public decimal TotalPayroll { get; set; }
        public decimal HighestSalary { get; set; }
        public decimal LowestSalary { get; set; }
        public decimal AverageSalary { get; set; }
    }

}
