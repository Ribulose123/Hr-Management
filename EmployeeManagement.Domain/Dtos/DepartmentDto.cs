

namespace EmployeeManagement.Domain.Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public List<EmployeeDto> Employees { get; set; } = new();
    }
}
