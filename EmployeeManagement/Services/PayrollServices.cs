using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EmployeeManagement.Services
{
    public class PayrollServices : IPayrollServices
    {
        private readonly EmployeeManagementDbContext _content;
        private readonly ILogger<PayrollServices> _logger;
        public double payPerDay = 10000;

        public PayrollServices(EmployeeManagementDbContext context, ILogger<PayrollServices> logger)
        {
            _content = context;
            _logger = logger;
        }


        //Generate payroll

        public async Task<(bool Success, string Message)> GeneratePayrollAsync(GeneratePayrollDto dto)
        {
            if (dto == null)
                return (false, "Invalid request. DTO is null.");

            // 1. Validate employee
            var employee = await _content.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.EmployerId);

            if (employee == null)
                return (false, "Employee does not exist.");

            // 2. Prevent duplicate payroll
            var existingPayroll = await _content.Payrolls
                .FirstOrDefaultAsync(p =>
                    p.EmployerId == dto.EmployerId &&
                    p.Month == dto.Month &&
                    p.year == dto.year);

            if (existingPayroll != null)
                return (false, "Payroll already exists for this month.");

            // 3. Salary check
            if (employee.BasicSalary <= 0)
                return (false, "Employee has no salary set.");

            // 4. Attendance
            var attendance = await _content.Attendances
                .Where(a =>
                    a.EmployeeId == dto.EmployerId &&
                    a.ClockInTime.Month == dto.Month &&
                    a.ClockInTime.Year == dto.year)
                .ToListAsync();

            // 5. Shift
            var shift = await _content.Shifts.FirstOrDefaultAsync(s => s.Id == employee.ShiftId);

            if (shift == null)
                return (false, "Employee has no shift assigned.");

            

            double expectedHoursPerDay =
                (shift.EndTime - shift.StartTime).TotalHours - (shift.BreakMinutes / 60.0);

            // Total hours worked
            double totalHoursWorked = attendance.Sum(a =>
                a.ClockOutTime != null ?
                (a.ClockOutTime.Value - a.ClockInTime).TotalHours : 0);

            // Total overtime
            double totalOvertimeHours = attendance.Sum(a =>
            {
                if (a.ClockOutTime == null) return 0;
                var hoursWorked = (a.ClockOutTime.Value - a.ClockInTime).TotalHours;
                return Math.Max(0, hoursWorked - expectedHoursPerDay);
            });

            // Total lateness
            double totalLateHours = attendance.Sum(a =>
                a.ClockInTime.TimeOfDay > shift.StartTime
                    ? (a.ClockInTime.TimeOfDay - shift.StartTime).TotalHours
                    : 0
            );

            // Absence
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

            // Net salary
            double netPay =
                employee.BasicSalary - (latenessDeduction + absenceDeduction) + overtimePay;


            // SAVE PAYROLL
            

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

            return (true, "Payroll generated successfully.");
        }


    }
}
