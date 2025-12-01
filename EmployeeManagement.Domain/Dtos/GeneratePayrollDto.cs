using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class GeneratePayrollDto
    {
        public int EmployerId { get; set; }
        public int Month { get; set; }
        public int year { get; set; }
    }
}
