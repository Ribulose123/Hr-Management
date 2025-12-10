using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class PayrollMonthRequestDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
