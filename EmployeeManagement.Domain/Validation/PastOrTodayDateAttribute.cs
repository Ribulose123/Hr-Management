using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Domain.Validation
{
    public class PastOrTodayDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.Date <= DateTime.Today;
            }

            return true;
        }
    }
}
