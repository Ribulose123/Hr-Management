using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollServices _payrollServices;

        public PayrollController (IPayrollServices payrollServices)
        {
            _payrollServices = payrollServices;
        }

        //Geneterate Payroll
        [HttpPost("generate-payroll")]
        public async Task<IActionResult> GeneratePayroll([FromBody] GeneratePayrollDto dto)
        {
            var result = await _payrollServices.GeneratePayrollAsync(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("employee/{employeeId}/payroll")]

        public async Task<IActionResult> GetEmployeePayroll(int employeeId, int year, int month)
        {
            var response = await _payrollServices.GetEmployeePayrollAsync(employeeId, year, month);

            if(!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Message);
        }


        //payroll summary

        [HttpGet("employee/{employeeId}/summary")]
        public async Task<IActionResult> GetPayrollSummary(int employeeId, int year)
        {
            var result = await _payrollServices.GetPayrollSummaryAsync(employeeId, year);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        //payroll history 
        [HttpGet("employee/{employeeId}/history")]
        public async Task<IActionResult> GetPayrollHistory(int employeeId)
        {
            var result = await _payrollServices.GetPayrollHistoryAsync(employeeId);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Data);
        }



    }
}
