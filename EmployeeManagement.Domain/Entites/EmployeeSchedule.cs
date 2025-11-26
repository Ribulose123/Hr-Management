using EmployeeManagement.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entites
{
    public class EmployeeSchedule
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public EmployeeDto? Employee { get; set; }
        public int ShiftId { get; set; }
        public Shift? Shift { get; set; }
        public DateTime WorkDate { get; set; }
        public bool IsAdjusted { get; set; } = false;
    }
}
