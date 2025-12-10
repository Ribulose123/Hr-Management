using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class SalaryStatsDto
    {
        public decimal TotalMonthlyPayroll { get; set; }
        public decimal HighestSalary { get; set; }
        public decimal LowestSalary { get; set; }
        public decimal AverageSalary { get; set; }
        public List<DepartmentPayrollDto>? PayrollForDepartment { get; set; }
    }
}
