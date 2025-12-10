using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class DepartmentPayrollDto
    {
        public string? DepartmentName { get; set; }
        public decimal TotalPayroll { get; set; }
    }
}
