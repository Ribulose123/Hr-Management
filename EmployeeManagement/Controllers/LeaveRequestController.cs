using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestServices _leaveRequestServices;
        public LeaveRequestController(ILeaveRequestServices leaveRequestServices)
        {
            _leaveRequestServices = leaveRequestServices;
        }
        // Apply for leave
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLeave([FromBody] ApplyLeaveDto dto)
        {
            var result = await _leaveRequestServices.ApplyForLeaveAsnyc(dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Data);
        }
    }
}
