using EmployeeManagement.Domain.Dtos;

namespace EmployeeManagement.Interfaces
{
    public interface IDashboardServices
    {
        Task<(bool Success, string Message, EmployeeStatsDto Data)> EmployeeStatAsync();
        Task<(bool Success, string Message, LeaveStatsDto Data)> LeaveStatAsync();
        //Task<(bool Success, string Message, SalaryStatsDto Data)> SalaryStatAsync();
        Task<(bool Success, string Message, PayrollMonthStatsDto? Data)> PayrollMonthStatAsync(PayrollMonthRequestDto dto);
    }
}
