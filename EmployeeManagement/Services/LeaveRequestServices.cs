using EmployeeManagement.Domain.Dtos;
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

            // Special Leave Rules

            if (dto.LeaveType == LeaveType.Maternity)
            {
                if (employee.Gender != Gender.Female)
                    return (false, "Only Female employees can apply", null);

                totalDays = 90;
                remainingLeave = 0;
                _logger.LogInformation("Maternity leave applied: 90 days enforced.");
            } else if (dto.LeaveType == LeaveType.Paternity)
            {
                if (employee.Gender != Gender.Male)
                    return (false, "Only male employees can apply", null);

                totalDays = 30;
                remainingLeave = 0;
                _logger.LogInformation("Maternity leave applied: 30 days enforced.");
            }
            else if (dto.LeaveType == LeaveType.Casual)
            {
                remainingLeave = employee.CasualLeave -totalDays;
            }

            //


            _logger.LogInformation("Employee {Id} AnnualLeave: {AnnualLeave}, ApprovedLeaveDays: {ApprovedLeaveDays}, RequestedDays: {RequestedDays}, RemainingLeave: {RemainingLeave}",
                employee.Id, employee.AnnualLeave, approvedLeaveDays, totalDays, remainingLeave);

            if (dto.LeaveType != LeaveType.Maternity && dto.LeaveType != LeaveType.Paternity)
            {
                if (totalDays > remainingLeave)
                    return (false, $"Insufficient {dto.LeaveType} leave balance.", null);
            } 

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
        public async Task<(bool Success, string Message, LeaveResponseDto? Data)> ApproveOrRejectLeave(LeaveApprovalDto dto)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == dto.LeaveId);

            if (leaveRequest == null)
                return (false, "Leave request not found", null);

            if (dto.Status == Status.Approved)
            {
                leaveRequest.Status = Status.Approved;

                // Deduct from the correct leave balance
                switch (leaveRequest.LeaveType)
                {
                    case LeaveType.Annual:
                        leaveRequest.Employee.AnnualLeave -= leaveRequest.TotalDays;
                        break;


                    case LeaveType.Casual:
                        leaveRequest.Employee.CasualLeave -= leaveRequest.TotalDays;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                leaveRequest.Status = Status.Rejected;
            }

            leaveRequest.ManagerComments = dto.ManagerComments;

            await _context.SaveChangesAsync();



            var leaveResponse = new LeaveResponseDto
            {
                FullName = $"{leaveRequest.Employee?.FirstName} {leaveRequest.Employee?.LastName}",
                Status = dto.Status,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
            };
            return (true, "Leave request updated", null);
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

        //Get leave balance

        public async Task<int> GetLeaveBalanceAsync(int employeeId)
        {
            var employeeExit = await _context.Employees.FindAsync(employeeId);
            if (employeeExit == null) return 0;

            var approvedLeaveDays = await _context.LeaveRequests
                .Where(l => l.EmployeeId == employeeId && l.Status == Status.Approved)
                .SumAsync(l => (int?)l.TotalDays) ?? 0;

            int currentBalance = (employeeExit.AnnualLeave) - approvedLeaveDays;

            return Math.Max(0, currentBalance);
        }
    }
}
