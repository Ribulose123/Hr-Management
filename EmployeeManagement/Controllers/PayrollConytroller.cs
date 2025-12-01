using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollConytroller : ControllerBase
    {
        private readonly IPayrollServices _payrollServices;

        public PayrollConytroller (IPayrollServices payrollServices)
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

    }
}
