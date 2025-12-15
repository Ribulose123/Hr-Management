using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
