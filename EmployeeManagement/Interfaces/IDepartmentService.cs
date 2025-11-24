using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;

namespace EmployeeManagement.Interfaces
{
    public interface IDepartmentService
    {
        Task<(bool Success, string Message)> DeleteDepartmentAsync(int id);
        Task<Department> CreateDepartmentAsync(DepartmentDto department);
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> UpdateDepartmentAsync(int id, Department updatedDepartment);
    }
}
