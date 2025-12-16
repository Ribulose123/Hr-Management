using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [HttpPost("apply-leave")]
        public async Task<IActionResult> ApplyForLeave([FromBody] ApplyLeaveDto dto)
        {
            var result = await _leaveRequestServices.ApplyForLeaveAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }



        //Leave request
        [Tags("Admin")]
        [Authorize (Roles = "Admin")]
        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests()
        {
            var list = await _leaveRequestServices.GetLeaveRequestsAsync();
            return Ok(list);
        }


        //LeaveApproval
        [Tags("Admin")]
        [Authorize (Roles = "Admin")]
        [HttpPost("approve-reject")]
        public async Task<IActionResult> ApproveOrRejectLeave([FromBody] LeaveApprovalDto dto)
        {
            var result = await _leaveRequestServices.ApproveOrRejectLeave(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }


        //LeaveBalance 
        
        [HttpGet("balance/{employeeId}")]
        public async Task<IActionResult> GetLeaveBalance(int employeeId)
        {
            var result = await _leaveRequestServices.GetLeaveBalanceAsync(employeeId);
            return Ok(new { LeaveBalance = result });
        }

    }
}
