using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class LeaveRequestServices:ILeaveRequestServices
    {
        private readonly ILogger<LeaveRequestServices> _logger;
        private readonly EmployeeManagementDbContext _context;
        public LeaveRequestServices(EmployeeManagementDbContext context, ILogger<LeaveRequestServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, LeaveResponseDto? Data)> ApplyForLeaveAsnyc(ApplyLeaveDto dto)
        {
            var employeeExit = await _context.Employees.FindAsync(dto.EmployeeId);

            if(employeeExit == null)
                return (false, "Employee does not exist.", null);

            if(dto.StartDate > dto.EndDate)
                return (false, "Start date cannot be after end date.", null);

            //check if the employee has enough annual leave 
            var totalDays = dto.EndDate.DayNumber - dto.StartDate.DayNumber + 1;
            if(employeeExit.AnnualLeave < totalDays)
                return (false, "Insufficient annual leave balance.", null);

            // Prevent overlapping leave requests

            var overLappingLeave = await _context.LeaveRequests.AnyAsync(
                I => I.EmployeeId == dto.EmployeeId && 
                I.Status == Domain.Enums.Status.Approved &&
                I.StartDate <= dto.EndDate &&
                 I.EndDate >= dto.StartDate);

            if(overLappingLeave)
                return (false, "Employee already has leave in this date range.", null);

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalDays = totalDays,
                LeaveType = dto.LeaveType,
                Status = Domain.Enums.Status.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();  

            var response = new LeaveResponseDto
            {
                LeaveId = leaveRequest.Id,
                EmployeeId = employeeExit.Id,
                FullName = $"{employeeExit.FirstName} {employeeExit.LastName}",
                LeaveType = leaveRequest.LeaveType,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                TotalDays = leaveRequest.TotalDays,
                Status = leaveRequest.Status,
                Remark = leaveRequest.ManagerComments
            };

            return (true, "Leave request submitted successfully.", response);
        }
    }
}
