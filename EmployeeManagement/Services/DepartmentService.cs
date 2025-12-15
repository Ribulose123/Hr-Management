using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly EmployeeManagementDbContext _context;
        private readonly ILogger<DepartmentService> _logger;
        private const string BackupDepartmentName = "Unassigned";

        public DepartmentService(EmployeeManagementDbContext context, ILogger<DepartmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> DeleteDepartmentAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var department = await _context.Departments
                    .Include(d => d.Employees)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (department == null)
                    return (false, "Department not found.");

                if (department.isDeleted)
                    return (false, "Department has already been deleted.");

                var backup = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentName == BackupDepartmentName);

                if (backup == null)
                {
                    backup = new Department { DepartmentName = BackupDepartmentName };
                    await _context.Departments.AddAsync(backup);
                    await _context.SaveChangesAsync();
                }

                if (department.Employees != null && department.Employees.Any())
                {
                    foreach (var emp in department.Employees)
                        emp.DepartmentId = backup.Id;
                }

                department.isDeleted = true;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Department '{department.DepartmentName}' deleted successfully.");
                return (true, $"Department '{department.DepartmentName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting department.");
                return (false, "An error occurred while deleting the department.");
            }
        }
        public async Task<Department> CreateDepartmentAsync(DepartmentDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
                throw new ArgumentException("Department name is required");

            bool exists = await _context.Departments
                .AnyAsync(d => d.DepartmentName == dto.DepartmentName);

            if (exists)
                throw new InvalidOperationException("Department already exists");

            var department = new Department
            {
                DepartmentName = dto.DepartmentName
            };

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            return department;
        }




        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .Where(d => !d.isDeleted)
                .ToListAsync();
        }

        public async Task<Department?> UpdateDepartmentAsync(int id, Department departments)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return null;

            department.DepartmentName = departments.DepartmentName;
            await _context.SaveChangesAsync();
            return department;
        }



    }
}
