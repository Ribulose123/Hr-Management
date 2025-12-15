using EmployeeManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public class Payroll
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public Employee? Employee { get; set; }
        public int Month { get; set; }
        public int year { get; set; }
        public double BasicSalary { get; set; }
        public double OvertimePay { get; set; }
        public double Deductions { get; set; }
        public double NetPay { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.Now;
    }
}
