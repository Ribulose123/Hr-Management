using EmployeeManagement.Domain.Dtos;

namespace EmployeeManagement.Interfaces
{
    public interface ILeaveRequestServices
    {
        Task <(bool Success, string Message, LeaveResponseDto? Data)> ApplyForLeaveAsync(ApplyLeaveDto dto);
        Task<List<LeaveResponseDto>> GetLeaveRequestsAsync();
        Task<(bool Success, string Message, LeaveResponseDto? Data)> ApproveOrRejectLeave(LeaveApprovalDto dto);
        Task<int> GetLeaveBalanceAsync(int employeeId);
    }
}
