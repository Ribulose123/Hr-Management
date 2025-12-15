

using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; }

        public bool isDeleted { get; set; } = false;
        public ICollection<Employee>? Employees { get; set; } = new List<Employee>();

    }
}
