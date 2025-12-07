using EmployeeManagement.Domain.Dtos;

namespace EmployeeManagement.Interfaces
{
    public interface IPayrollServices
    {
        Task<(bool Success, string Message, PayrollResponseDto? Data)> GeneratePayrollAsync(GeneratePayrollDto dto);

        Task <(bool Success, string Message, PayrollResponseDto? Data)> GetEmployeePayrollAsync( int employeeId, int month, int year );
        Task<(bool Success, string Message, PayrollSummaryDto? Data)> GetPayrollSummaryAsync(int employeeId, int year);

        Task<(bool Success, string Message, List<PayrollResponseDto>? Data)>
    GetPayrollHistoryAsync(int employeeId);

    }
}
