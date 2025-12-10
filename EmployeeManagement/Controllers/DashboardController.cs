using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardServices _dashboardServices;

        public DashboardController(IDashboardServices dashboardServices)
        {
            _dashboardServices = dashboardServices;
        }

        [HttpGet("EmployeeState")]
        public async Task<IActionResult> GetEmployeeStats()
        {
            var result = await _dashboardServices.EmployeeStatAsync();

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            });
        }

        [HttpGet ("LeaveStat")]
        public async Task<IActionResult> GetLeaveStats()
        {
            var result = await _dashboardServices.LeaveStatAsync();

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            });
        }

        //Get Salary by month

        [HttpPost ("SalaryMonthlyStat")]

        public async Task<IActionResult> GetSalaryMonthlyStat([FromBody] PayrollMonthRequestDto dto)
        {
            var result = await _dashboardServices.PayrollMonthStatAsync( dto);
            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            });
        }

        //Get Salary Stat
        [HttpGet("SalaryStat")]

        public async Task<IActionResult> GetSalaryStat()
        {
            var result = await _dashboardServices.SalaryStatAsync();
            if (!result.Success)
                return BadRequest(new { result.Message });
            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            });
        }
    }
}
