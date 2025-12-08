using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
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

        public async Task<(bool Success, string Message, LeaveResponseDto? Data)> ApplyForLeaveAsync(ApplyLeaveDto dto)
        {
            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
                return (false, "Employee does not exist.", null);

            if (dto.StartDate > dto.EndDate)
                return (false, "Start date cannot be after end date.", null);

            var totalDays = (dto.EndDate.ToDateTime(TimeOnly.MinValue) - dto.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            if (totalDays <= 0)
                return (false, "Invalid leave period.", null);

            var approvedLeaveDays = await _context.LeaveRequests
                .Where(l => l.EmployeeId == dto.EmployeeId && l.Status == Status.Approved)
                .SumAsync(l => l.TotalDays);

            var remainingLeave = employee.AnnualLeave - approvedLeaveDays;

            _logger.LogInformation("Employee {Id} AnnualLeave: {AnnualLeave}, ApprovedLeaveDays: {ApprovedLeaveDays}, RequestedDays: {RequestedDays}, RemainingLeave: {RemainingLeave}",
                employee.Id, employee.AnnualLeave, approvedLeaveDays, totalDays, remainingLeave);

            if (totalDays > remainingLeave)
                return (false, "Insufficient annual leave balance.", null);

            var overlappingLeave = await _context.LeaveRequests.AnyAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.Status == Status.Approved &&
                l.StartDate <= dto.EndDate &&
                l.EndDate >= dto.StartDate);

            if (overlappingLeave)
                return (false, "Employee already has leave in this date range.", null);

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalDays = totalDays,
                LeaveType = dto.LeaveType,
                Status = Status.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();

            var response = new LeaveResponseDto
            {
                LeaveId = leaveRequest.Id,
                EmployeeId = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                LeaveType = leaveRequest.LeaveType,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                TotalDays = leaveRequest.TotalDays,
                Status = leaveRequest.Status,
                ManagerComments = leaveRequest.ManagerComments
            };

            return (true, "Leave request submitted successfully.", response);
        }
        //List of request 

        public async Task<List<LeaveResponseDto>> GetLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
        .Include(l => l.Employee)
        .Select(l => new LeaveResponseDto
        {
            LeaveId = l.Id,
            EmployeeId = l.EmployeeId,
            FullName = l.Employee != null ? l.Employee.FirstName + " " + l.Employee.LastName : null,
            LeaveType = l.LeaveType,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            TotalDays = l.TotalDays,
            Status = l.Status,
            ManagerComments = l.ManagerComments
        })
        .ToListAsync();
        }
    }
}
