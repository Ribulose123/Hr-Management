using EmployeeManagement.Domain.Dtos;

namespace EmployeeManagement.Interfaces
{
    public interface ILeaveRequestServices
    {
        Task <(bool Success, string Message, LeaveResponseDto? Data)> ApplyForLeaveAsync(ApplyLeaveDto dto);
        Task<List<LeaveResponseDto>> GetLeaveRequestsAsync();
    }
}
