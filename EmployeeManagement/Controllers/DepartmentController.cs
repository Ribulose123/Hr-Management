using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var (success, message) = await _departmentService.DeleteDepartmentAsync(id);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto department)
        {
            var created = await _departmentService.CreateDepartmentAsync(department);
            return CreatedAtAction(nameof(GetAllDepartments), new { id = created.Id }, created);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] Department department)
        {
            var updated = await _departmentService.UpdateDepartmentAsync(id, department);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
}
}
