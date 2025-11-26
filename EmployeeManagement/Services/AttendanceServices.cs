using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class AttendanceServices:IAttendanceService
    {
        private readonly EmployeeManagementDbContext _context;
        private readonly ILogger<AttendanceServices> _logger;
        public AttendanceServices(EmployeeManagementDbContext context, ILogger<AttendanceServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        //ClockIn
        public async Task<(bool Success, string Message)> ClockInAsync (ClockInDto dto)
        {
            bool employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);

            if(!employeeExists)
                return (false, "Employee does not exist.");

            var schedule = await _context.EmployeeSchedules.Include(e => e.Shift).FirstOrDefaultAsync((es => es.EmployeeId ==dto.EmployeeId && es.WorkDate.Date == DateTime.Today));

            if(schedule == null)
                return (false, "You are not scheduled to work today.");

            if (schedule.WorkDate.Date > DateTime.Today)
                return (false, "you are late");

            var existingRecord = await _context.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.ClockInTime != null);

            if(existingRecord != null) 
                return (false, "Employee has already clocked in.");

            DateTime sheduleDate = schedule.WorkDate.Date + schedule.Shift.StartTime;
            DateTime now = DateTime.Now;

            string status;

            if (now > sheduleDate)
            {
                status = "Late";
            } else if(now < sheduleDate)
            {
                status = "Early";
            }    else
            {
                status = "On Time";
            }

            var attendance = new Attendance
        {
            EmployeeId = dto.EmployeeId,
            ClockInTime = DateTime.Now
        };

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();

            return (true, $"Clock-in successful. Status: {status}");
        }

        //ClockOut

        public async Task<(bool Success, string Message)> ClockOutAsync(ClockOutDto dto)
        {
            int employeeId = dto.EmployeeId;
            // 1. Check if employee exists
            bool employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);
            if (!employeeExists)
                return (false, "Employee does not exist.");

            // 2. Get schedule for today
            var schedule = await _context.EmployeeSchedules
                .Include(es => es.Shift)
                .FirstOrDefaultAsync(es =>
                    es.EmployeeId == employeeId &&
                    es.WorkDate.Date == DateTime.Today);

            if (schedule == null)
                return (false, "You are not scheduled to work today.");

            var shiftStart = schedule.WorkDate.Date.Add(schedule.Shift.StartTime);
            var shiftEnd = schedule.WorkDate.Date.Add(schedule.Shift.EndTime);

            // 3. Get today's attendance record
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.ClockInTime.Date == DateTime.Today);

            if (attendance == null)
                return (false, "You have not clocked in.");

            // Prevent double clock-out
            if (attendance.ClockOutTime != null)
                return (false, "You have already clocked out.");

            // 4. Clock-out
            attendance.ClockOutTime = DateTime.Now;

            // 5. Calculate total work hours
            attendance.TotalHours = (attendance.ClockOutTime.Value - attendance.ClockInTime).TotalHours;

            // 6. Determine Early Exit / On-time / Overtime
            string statusMessage;

            if (attendance.ClockOutTime < shiftEnd)
            {
                statusMessage = "Clock-out successful. You ended **early**.";
            }
            else if (attendance.ClockOutTime >= shiftEnd && attendance.ClockOutTime <= shiftEnd.AddMinutes(10))
            {
                // 0–10 min late is considered on-time for exit
                statusMessage = "Clock-out successful. You ended **on time**.";
            }
            else
            {
                // After shift end = overtime
                var overtime = (attendance.ClockOutTime.Value - shiftEnd).TotalHours;

                statusMessage = $"Clock-out successful. **Overtime: {overtime:F2} hrs**.";
            }

            await _context.SaveChangesAsync();

            return (true, statusMessage);
        }



        //Get Attendance Records
        public async Task<IEnumerable<Attendance>> GetAttendanceByEmployeeAsync(int employeeId)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId).Include(e =>e.Employee)
                .OrderByDescending(a => a.ClockInTime)
                .ToListAsync();
        }

    }
}
