using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagement.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly EmployeeManagementDbContext _context;
        private readonly ILogger<EmployeeServices> _logger;
        private const string BackupDepartmentName = "Unassigned";

        public EmployeeServices(EmployeeManagementDbContext context, ILogger<EmployeeServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Create Employee
        public async Task<(bool Success, string Message, Employee? Employee)> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            if (dto == null)
                return (false, "Invalid employee data.", null);

            // Check if department exists and is not deleted
            var departmentExists = await _context.Departments
                .AnyAsync(e => e.Id == dto.DepartmentId && !e.isDeleted);

            // If department doesn’t exist, use backup
            if (!departmentExists)
            {
                var backup = await _context.Departments
                    .FirstOrDefaultAsync(e => e.DepartmentName == BackupDepartmentName);

                if (backup == null)
                {
                    backup = new Department { DepartmentName = BackupDepartmentName };
                    await _context.Departments.AddAsync(backup);
                    await _context.SaveChangesAsync();
                }

                dto.DepartmentId = backup.Id;
            }

            var employee = new Employee
            {
               FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Position = dto.Position,
                BasicSalary = dto.BasicSalary,
                HireDate = dto.HireDate,
                IsActive = dto.IsActive,
                DepartmentId = dto.DepartmentId
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Employee '{employee.FirstName}' created successfully.");

            var employeeWithDepartment = await _context.Employees.Include(e => e.AssignedDepartment).FirstOrDefaultAsync(e => e.Id == employee.Id);

            return (true, "Employee created successfully.", employeeWithDepartment);
        }

        // ✅ Update Employee’s Department
        public async Task<(bool Success, string Message, Employee? Employee)> UpdateEmployeeAsync(int id, UpdateEmployeeDepartmentDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return (false, "Employee not found.", null);

            // Check department
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId && !d.isDeleted);

            if (department == null)
                return (false, "Invalid department.", null);

            employee.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();

            return (true, "Employee updated successfully.", employee);
        }


        // ✅ Get All Employees
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.AssignedDepartment)
                .ToListAsync();
        }

        // ✅ Delete Employee
        public async Task<(bool Success, string Message)> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return (false, "Employee not found.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Employee '{employee.FirstName}' deleted successfully.");
            return (true, "Employee deleted successfully.");
        }
    }
}
