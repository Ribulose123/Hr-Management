using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceContoller : ControllerBase
    {
        private readonly AttendanceServices _attendanceServices;

        public AttendanceContoller(AttendanceServices attendanceServices)
        {
            _attendanceServices = attendanceServices;
        }

        [HttpPost("clock-in")]
        public async Task<IActionResult> ClockIn(ClockInDto dto)
        {
            var result = await _attendanceServices.ClockInAsync(dto);
            return Ok(result);
        }
        [HttpPost("clock-out")]

        public async Task<IActionResult> ClockOut(ClockOutDto dto)
        {
            var result = await _attendanceServices.ClockOutAsync(dto);
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAttendance(int employeeId)
        {
            var records = await _attendanceServices.GetAttendanceByEmployeeAsync(employeeId);
            return Ok(records);
        }
    }
}
