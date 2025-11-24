using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;

namespace EmployeeManagement.Interfaces
{
    public interface IAttendanceService
    {
        Task<(bool Success, string Message)> ClockInAsync(ClockInDto dto);
        Task<(bool Success, string Message)> ClockOutAsync(ClockOutDto dto);
        Task<IEnumerable<Attendance>> GetAttendanceByEmployeeAsync(int employeeId);
    }

}
