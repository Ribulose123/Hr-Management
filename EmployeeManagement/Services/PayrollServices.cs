using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class PayrollServices : IPayrollServices
    {
        private readonly EmployeeManagementDbContext _content;
        private readonly ILogger<PayrollServices> _logger;

        public PayrollServices(EmployeeManagementDbContext context, ILogger<PayrollServices> logger)
        {
            _content = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, PayrollResponseDto? Data)> GeneratePayrollAsync(GeneratePayrollDto dto)
        {
            if (dto == null)
                return (false, "Invalid request. DTO is null.", null);

            // 1. Validate employee
            var employee = await _content.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.EmployerId);

            if (employee == null)
                return (false, "Employee does not exist.", null);

            // 2. Prevent duplicate payroll
            var existingPayroll = await _content.Payrolls
                .FirstOrDefaultAsync(p =>
                    p.EmployerId == dto.EmployerId &&
                    p.Month == dto.Month &&
                    p.year == dto.year);

            if (existingPayroll != null)
                return (false, "Payroll already exists for this month.", null);

            // 3. Salary check
            if (employee.BasicSalary <= 0)
                return (false, "Employee has no salary set.", null);

            // 4. Attendance
            var attendance = await _content.Attendances
                .Where(a =>
                    a.EmployeeId == dto.EmployerId &&
                    a.ClockInTime.Month == dto.Month &&
                    a.ClockInTime.Year == dto.year)
                .ToListAsync();

            // 5. Shift
            var shift = await _content.Shifts
                .FirstOrDefaultAsync(s => s.Id == employee.ShiftId);

            if (shift == null)
                return (false, "Employee has no shift assigned.", null);

            // Expected hours per day
            double expectedHoursPerDay =
                (shift.EndTime - shift.StartTime).TotalHours - (shift.BreakMinutes / 60.0);

            // Total hours worked
            double totalHoursWorked = attendance.Sum(a =>
                a.ClockOutTime != null ? (a.ClockOutTime.Value - a.ClockInTime).TotalHours : 0);

            // Overtime
            double totalOvertimeHours = attendance.Sum(a =>
            {
                if (a.ClockOutTime == null) return 0;
                var worked = (a.ClockOutTime.Value - a.ClockInTime).TotalHours;
                return Math.Max(0, worked - expectedHoursPerDay);
            });

            // Lateness hours
            double totalLateHours = attendance.Sum(a =>
                a.ClockInTime.TimeOfDay > shift.StartTime
                    ? (a.ClockInTime.TimeOfDay - shift.StartTime).TotalHours
                    : 0);

            // Absence calculation
            int totalDaysInMonth = DateTime.DaysInMonth(dto.year, dto.Month);
            int presentDays = attendance.Count(a => a.ClockOutTime != null);
            int absentDays = totalDaysInMonth - presentDays;

            // Salary components
            double payPerDay = employee.BasicSalary / totalDaysInMonth;
            double hourlyRate = employee.BasicSalary / (totalDaysInMonth * expectedHoursPerDay);

            // Overtime pay
            double overtimePay = totalOvertimeHours * (hourlyRate * 1.5);

            // Deductions
            double latenessDeduction = (totalLateHours / expectedHoursPerDay) * payPerDay;
            double absenceDeduction = absentDays * payPerDay;

            // Net Pay
            double netPay = employee.BasicSalary
                            - (latenessDeduction + absenceDeduction)
                            + overtimePay;

            // Save to DB
            var payroll = new Payroll
            {
                EmployerId = employee.Id,
                Month = dto.Month,
                year = dto.year,
                BasicSalary = employee.BasicSalary,
                OvertimePay = overtimePay,
                Deductions = latenessDeduction + absenceDeduction,
                NetPay = netPay
            };

            _content.Payrolls.Add(payroll);
            await _content.SaveChangesAsync();

            // Prepare response DTO
            var response = new PayrollResponseDto
            {
                EmployeeId = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Month = dto.Month,
                Year = dto.year,
                BasicSalary = employee.BasicSalary,
                TotalHoursWorked = totalHoursWorked,
                OvertimeHours = totalOvertimeHours,
                OvertimePay = overtimePay,
                LateHours = totalLateHours,
                AbsenceDays = absentDays,
                AbsenceDeduction = absenceDeduction,
                LatenessDeduction = latenessDeduction,
                GrossPay = employee.BasicSalary + overtimePay,
                NetPay = netPay
            };

            return (true, "Payroll generated successfully.", response);
        }


        public async Task<(bool Success, string Message, PayrollResponseDto? Data)> GetEmployeePayrollAsync(int employeeId, int year, int month)
        {
            var payrollExit = await _content.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p =>
                    p.EmployerId == employeeId &&
                    p.year == year &&
                    p.Month == month);

            if (payrollExit == null)
                return (false, "No payroll was found", null);

            var dto = new PayrollResponseDto
            {
                EmployeeId = payrollExit.EmployerId,
                FullName = $"{payrollExit.Employee!.FirstName} {payrollExit.Employee.LastName}",
                Month = payrollExit.Month,
                Year = payrollExit.year,
                OvertimePay = payrollExit.OvertimePay,
                AbsenceDeduction = payrollExit.Deductions,
                BasicSalary = payrollExit.BasicSalary,
                NetPay = payrollExit.NetPay,
            };

            return (true, "Payroll retrieved successfully.", dto);
        }


        public async Task<(bool Success, string Message, PayrollSummaryDto? Data)>
     GetPayrollSummaryAsync(int employeeId, int year)
        {
            var payrolls = await _content.Payrolls
                .Where(p => p.EmployerId == employeeId && p.year == year)
                .ToListAsync();

            if (!payrolls.Any())
                return (false, "No payroll records for this year.", null);

            var summary = new PayrollSummaryDto
            {
                EmployeeId = employeeId,
                Year = year,
                TotalBasicSalary = payrolls.Sum(p => p.BasicSalary),
                TotalOvertimePay = payrolls.Sum(p => p.OvertimePay),
                TotalDeductions = payrolls.Sum(p => p.Deductions),
                TotalNetPay = payrolls.Sum(p => p.NetPay)
            };

            return (true, "Payroll summary loaded.", summary);
        }

        public async Task<(bool Success, string Message, List<PayrollResponseDto>? Data)>
    GetPayrollHistoryAsync(int employeeId)
        {
            var payrolls = await _content.Payrolls
                .Include(p => p.Employee)
                .Where(p => p.EmployerId == employeeId)
                .OrderByDescending(p => p.year)
                .ThenByDescending(p => p.Month)
                .ToListAsync();

            if (!payrolls.Any())
                return (false, "No payroll history found.", null);

            var list = payrolls.Select(p => new PayrollResponseDto
            {
                EmployeeId = p.EmployerId,
                FullName = p.Employee!.FirstName,
                Month = p.Month,
                Year = p.year,
                BasicSalary = p.BasicSalary,
                OvertimePay = p.OvertimePay,
                AbsenceDeduction = p.Deductions,
                NetPay = p.NetPay
            }).ToList();

            return (true, "Payroll history retrieved.", list);
        }


    }
}
