using EmployeeManagement.Domain.Dtos;

namespace EmployeeManagement.Interfaces
{
    public interface IPayrollServices
    {
        Task<(bool Success, string Message)> GeneratePayrollAsync(GeneratePayrollDto dto);
    }
}
