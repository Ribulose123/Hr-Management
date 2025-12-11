using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class DashboardServices:IDashboardServices
    {
        private readonly EmployeeManagementDbContext _context;
        private readonly ILogger<DashboardServices> _logger;

        public DashboardServices (EmployeeManagementDbContext context, ILogger<DashboardServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Get all Employee stat

        public async Task<(bool Success, string Message, EmployeeStatsDto Data)> EmployeeStatAsync()
        {
            var totalEmployees = await _context.Employees.CountAsync();
            var activeEmployees = await _context.Employees.CountAsync(e => e.IsActive);
            var inactiveEmployees = totalEmployees - activeEmployees;

            var data = new EmployeeStatsDto
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                InactiveEmployees = inactiveEmployees
            };

            return (true, "Employee statistics fetched successfully.", data);

        }

        //Get all leave stat

        public async Task<(bool Success, string Message, LeaveStatsDto Data)> LeaveStatAsync()
        {
            var totalLeaveRequest = await _context.LeaveRequests.CountAsync();
            var pendingLeaveRequest = await _context.LeaveRequests.CountAsync(e => e.Status == Domain.Enums.Status.Pending);
            var approvedLeaveRequest = await _context.LeaveRequests.CountAsync(e => e.Status == Domain.Enums.Status.Approved);
            var rejectedLeaveRequest = await _context.LeaveRequests.CountAsync(e => e.Status == Domain.Enums.Status.Rejected);
            var totalAnnualLeaveRequest = await _context.LeaveRequests.CountAsync(e => e.LeaveType == Domain.Enums.LeaveType.Annual);
            var totalMaternityLeaveRequest = await _context.LeaveRequests.CountAsync(e => e.LeaveType == Domain.Enums.LeaveType.Maternity);
            var totalPaternityLeaveRequest = await _context.LeaveRequests.CountAsync(e => e.LeaveType == Domain.Enums.LeaveType.Paternity);
            var totalCasualLeaveReqest = await _context.LeaveRequests.CountAsync(e => e.LeaveType == Domain.Enums.LeaveType.Casual);

            var leaveData = new LeaveStatsDto
            {
                TotalLeaveRequests =totalAnnualLeaveRequest,
                PendingRequests = pendingLeaveRequest,
                ApprovedRequests = approvedLeaveRequest,
                RejectedRequests = rejectedLeaveRequest,
            };

            return (true, "Leave statistics fetched successfully.", leaveData);
        }

        // Get monthly salary Stat

        public async Task<(bool Success, string Message, PayrollMonthStatsDto? Data)> PayrollMonthStatAsync(PayrollMonthRequestDto dto)
        {
            // Total employees
            var totalEmployees = await _context.Employees.CountAsync();

            var payroll = _context.Payrolls
                .Where(e => e.Month == dto.Month && e.year == dto.Year);

            if (!await payroll.AnyAsync())
                return (false, "No payroll found", null);

            var totalPayroll = (decimal)await payroll.SumAsync(p => p.NetPay);
            var highestSalary = (decimal)await payroll.MaxAsync(p => p.NetPay);
            var lowestSalary = (decimal)await payroll.MinAsync(p => p.NetPay);
            var averageSalary = (decimal)await payroll.AverageAsync(p => p.NetPay);

            var result = new PayrollMonthStatsDto
            {
                TotalEmployees = totalEmployees,
                TotalPayroll = totalPayroll,
                HighestSalary = highestSalary,
                LowestSalary = lowestSalary,
                AverageSalary = averageSalary
            };

            return (true, "Payroll statistics fetched successfully.", result);
        }


        // Get Salary Stat
        public async Task<(bool Success, string Message, SalaryStatsDto Data)> SalaryStatAsync()
        {
            // Total net salary
            var totalSalary = await _context.Payrolls.SumAsync(e => e.NetPay);
           var highestSalary = await _context.Payrolls.MaxAsync(e => e.NetPay);
            var lowestSalary = await _context.Payrolls.MinAsync(e => e.NetPay);
            var averageSalary = await _context.Payrolls.AverageAsync(p => p.NetPay);

            // MUST load Employees + Departments + Payrolls to memory first
            var employeesWithPayroll = await _context.Employees
                .Include(e => e.AssignedDepartment)
                .GroupJoin(
                    _context.Payrolls,
                    e => e.Id,
                    p => p.EmployerId,
                    (employee, payrolls) => new { employee, payrolls }
                )
                .AsNoTracking()
                .ToListAsync(); 

            var payrollByDepartment = employeesWithPayroll
                .GroupBy(e => e.employee.AssignedDepartment?.DepartmentName ?? "Unassigned")
                .Select(g => new DepartmentPayrollDto
                {
                    DepartmentName = g.Key,
                    TotalPayroll =(decimal) g.Sum(x => x.payrolls.Sum(p => p.NetPay))
                })
                .ToList();

            var salaryData = new SalaryStatsDto
            {
                TotalMonthlyPayroll =(decimal) totalSalary,
                HighestSalary = (decimal)highestSalary,
                LowestSalary = (decimal)lowestSalary,
                AverageSalary = (decimal)averageSalary,
                PayrollForDepartment = payrollByDepartment
            };

            return (true, "Salary statistics fetched successfully.", salaryData);
        }

        // Get Department Stat
        public async Task<(bool Success, string Message, DepartmentStatsDto Data)> DepartmentStatAsync()
        {
            var totalDepartments = await _context.Departments.CountAsync();
            var DepartmentCounts = await _context.Departments.Select(d => new DepartmentCountDto
            {
                DepartmentName = d.DepartmentName,
                EmployeeCount = _context.Employees.Count(e => e.DepartmentId == d.Id)
            }).AsNoTracking().ToListAsync();

            var departmentData = new DepartmentStatsDto
            {
                TotalDepartments = totalDepartments,
                DepartmentCounts = DepartmentCounts
            };
            return (true , "Department statistics fetched successfully.", departmentData);
        }
    }
}
