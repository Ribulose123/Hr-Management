using EmployeeManagement.Domain.Entites;
using EmployeeManagement.Domain.Validation;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee
    {
        public int Id {get; set;}
        public string? FirstName {get; set;}
        public string? LastName {get; set;}
        public string? Email {get; set;}
        public string? PhoneNumber {get; set;}
        public string? Position {get; set;}
        public double BasicSalary {get; set;}
        [PastOrTodayDate(ErrorMessage = "Date of hire must be today or past")]
        public DateTime HireDate {get; set; }
        public bool IsActive {get; set; }
        public int DepartmentId { get; set; }
        public Department? AssignedDepartment { get; set; }
        public int ShiftId { get; set; }
        public Shift? Shift { get; set; }
        public int AnnualLeave { get; set; } = 20;

    }
}
