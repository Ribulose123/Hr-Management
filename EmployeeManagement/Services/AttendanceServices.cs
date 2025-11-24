using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
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

            var existingRecord = await _context.Attendances.Where(a => a.EmployeeId == dto.EmployeeId && a.ClockInTime == null).FirstOrDefaultAsync();

            if(existingRecord != null) 
                return (false, "Employee has already clocked in.");

            var attendance = new Attendance
            {
                EmployeeId = dto.EmployeeId,
                ClockInTime = DateTime.Now
            };

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();

            return (true, "Clock-in successful.");
        }

        //ClockOut

        public async Task<(bool Success, string Message)> ClockOutAsync(ClockOutDto dto)
        {
            // Check employee exists
            bool exists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
            if (!exists)
                return (false, "Employee not found.");


            var existingRecord = await _context.Attendances.Where(a => a.EmployeeId == dto.EmployeeId && a.ClockOutTime == null).FirstOrDefaultAsync();

            if (existingRecord == null)
                return (false, "No clock-in record found for the employee.");

            existingRecord.ClockOutTime = DateTime.Now;
            existingRecord.TotalHours = (existingRecord.ClockOutTime.Value - existingRecord.ClockInTime).TotalHours;

            await _context.SaveChangesAsync();
            return (true, "Clock-out successful.");
        }


        //Get Attendance Records
        public async Task<IEnumerable<Attendance>> GetAttendanceByEmployeeAsync(int employeeId)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.ClockInTime)
                .ToListAsync();
        }

    }
}
