using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Persistence.IdentityError
{
    public class CustomIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override Microsoft.AspNetCore.Identity.IdentityError DuplicateEmail(string email)
        {
            return new Microsoft.AspNetCore.Identity.IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"The email '{email}' is already registered. Please use a different email address."
            };
        }

        public override Microsoft.AspNetCore.Identity.IdentityError PasswordTooShort(int length)
        {
            return new Microsoft.AspNetCore.Identity.IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"The password is too short. It must be at least {length} characters long."
            };
        }
    }
}
