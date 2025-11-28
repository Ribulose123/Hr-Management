using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class AttendanceServices : IAttendanceService
    {
        private readonly EmployeeManagementDbContext _context;
        private readonly ILogger<AttendanceServices> _logger;

        public AttendanceServices(EmployeeManagementDbContext context, ILogger<AttendanceServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // 1️⃣ ENSURE DEFAULT SHIFT EXISTS (8am – 5pm)
        // ---------------------------------------------------------
        private async Task<int> EnsureDefaultShiftExists()
        {
            var shift = await _context.Shifts
                .FirstOrDefaultAsync(s => s.Name == "Default Work Shift");

            if (shift == null)
            {
                shift = new Shift
                {
                    Name = "Default Work Shift",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    BreakMinutes = 60
                };

                _context.Shifts.Add(shift);
                await _context.SaveChangesAsync();
            }

            return shift.Id;
        }

        // ---------------------------------------------------------
        // 2️⃣ ENSURE DAILY SCHEDULE FOR EMPLOYEE
        // ---------------------------------------------------------
        private async Task<EmployeeSchedule> EnsureScheduleForTodayAsync(int employeeId)
        {
            var today = DateTime.Today;

            var schedule = await _context.EmployeeSchedules
                .Include(es => es.Shift)
                .FirstOrDefaultAsync(es =>
                    es.EmployeeId == employeeId &&
                    es.WorkDate.Date == today);

            if (schedule != null)
                return schedule;

            int defaultShiftId = await EnsureDefaultShiftExists();

            schedule = new EmployeeSchedule
            {
                EmployeeId = employeeId,
                WorkDate = today,
                ShiftId = defaultShiftId
            };

            _context.EmployeeSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Reload with Shift object included
            return await _context.EmployeeSchedules
                .Include(es => es.Shift)
                .FirstAsync(es => es.Id == schedule.Id);
        }

        // ---------------------------------------------------------
        // 3️⃣ CLOCK IN
        // ---------------------------------------------------------
        public async Task<(bool Success, string Message)> ClockInAsync(ClockInDto dto)
        {
            if (!await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId))
                return (false, "Employee does not exist.");

            var schedule = await EnsureScheduleForTodayAsync(dto.EmployeeId);
            if (schedule == null)
                return (false, "Schedule could not be created for today.");

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == dto.EmployeeId &&
                    a.ClockInTime.Date == DateTime.Today);

            if (existing != null)
                return (false, "You have already clocked in.");

            DateTime shiftStart = schedule.WorkDate.Date.Add(schedule.Shift.StartTime);
            DateTime shiftEnd = schedule.WorkDate.Date.Add(schedule.Shift.EndTime);

            // 60-minute grace period
            var gracePeriod = TimeSpan.FromMinutes(60);
            DateTime graceLimit = shiftStart.Add(gracePeriod);

            DateTime now = DateTime.Now;

            // ❌ No clock-in after closing hours
            if (now > shiftEnd)
                return (false, "Clock-in failed. You cannot clock in after closing hours.");

            string status;

            if (now < shiftStart)
            {
                status = "Early";   // before start time
            }
            else if (now > shiftStart && now <= graceLimit)
            {
                status = "Late (within grace period)";
            }
            else if(now > graceLimit && now <= shiftEnd)
            {
                status = "Late";    // after grace period
            } else
            {
                status = "On Time";
            }

            var attendance = new Attendance
            {
                EmployeeId = dto.EmployeeId,
                ClockInTime = now,
                Status = status
            };

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();

            return (true, $"Clock-in successful. Status: {status}");
        }


        // ---------------------------------------------------------
        // 4️⃣ CLOCK OUT
        // ---------------------------------------------------------
        public async Task<(bool Success, string Message)> ClockOutAsync(ClockOutDto dto)
        {
            int employeeId = dto.EmployeeId;

            if (!await _context.Employees.AnyAsync(e => e.Id == employeeId))
                return (false, "Employee does not exist.");

            // Get today's schedule
            var schedule = await EnsureScheduleForTodayAsync(employeeId);

            DateTime shiftEnd = schedule.WorkDate.Date.Add(schedule.Shift.EndTime);

            // Get today's attendance
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.ClockInTime.Date == DateTime.Today);

            if (attendance == null)
                return (false, "You have not clocked in today.");

            if (attendance.ClockOutTime != null)
                return (false, "You have already clocked out.");

            // Perform clock-out
            attendance.ClockOutTime = DateTime.Now;
            attendance.TotalHours =
                (attendance.ClockOutTime.Value - attendance.ClockInTime).TotalHours;

            // Determine clock-out status
            string statusMessage;

            if (attendance.ClockOutTime < shiftEnd)
            {
                statusMessage = "Clock-out successful. You ended early.";
            }
            else if (attendance.ClockOutTime <= shiftEnd.AddMinutes(10))
            {
                statusMessage = "Clock-out successful. You ended on time.";
            }
            else
            {
                var overtime = (attendance.ClockOutTime.Value - shiftEnd).TotalHours;
                statusMessage = $"Clock-out successful. Overtime: {overtime:F2} hours.";
            }

            await _context.SaveChangesAsync();
            return (true, statusMessage);
        }

        // 5️⃣ GET ATTENDANCE HISTORY
        // ---------------------------------------------------------
        public async Task<IEnumerable<Attendance>> GetAttendanceByEmployeeAsync(int employeeId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.ClockInTime)
                .ToListAsync();
        }
    }
}
