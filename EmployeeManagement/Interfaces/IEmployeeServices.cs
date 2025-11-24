using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Interfaces
{
    public interface IEmployeeServices
    {
        Task<(bool Success, string Message, Employee? Employee)> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<(bool Success, string Message, Employee? Employee)> UpdateEmployeeAsync(int id, UpdateEmployeeDepartmentDto dto);

        Task<List<Employee>> GetEmployeesAsync();
        Task<(bool Success, string Message)> DeleteEmployeeAsync(int id);
    }
}
