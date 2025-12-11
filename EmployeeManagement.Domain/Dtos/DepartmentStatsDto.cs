using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Dtos
{
    public class DepartmentStatsDto
    {
        public int TotalDepartments { get; set; }
        public List<DepartmentCountDto>? DepartmentCounts { get; set; }
    }
}
