using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceContoller : ControllerBase
    {
        private readonly IAttendanceService _attendanceServices;

        public AttendanceContoller(IAttendanceService attendanceServices)
        {
            _attendanceServices = attendanceServices;
        }

        [HttpPost("clock-in")]
        public async Task<IActionResult> ClockIn([FromBody] ClockInDto dto)
        {
            try
            {
                var (success, message) = await _attendanceServices.ClockInAsync(dto);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Clock-In ERROR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("clock-out")]
        public async Task<IActionResult> ClockOut([FromBody] ClockOutDto dto)
        {
            try
            {
                var (success, message) = await _attendanceServices.ClockOutAsync(dto);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Clock-Out ERROR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAttendance(int employeeId)
        {
            try
            {
                var records = await _attendanceServices.GetAttendanceByEmployeeAsync(employeeId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Attendance Fetch ERROR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
