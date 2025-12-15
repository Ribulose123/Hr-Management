using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using JwtClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public class AuthenticationService: IAuthenticationServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmployeeManagementDbContext _content;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmployeeManagementDbContext context, IConfiguration configuration)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _content = context;
            _configuration = configuration;
        }


        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>

    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email!)
    };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:ExpirationInHours"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //Regisration

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            var employeeExist = await _content.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);

            if (employeeExist == null)
                return (false, "No Employee found");


            // Checking if Email has been used before

            var checkingEmail = await _userManager.FindByEmailAsync(dto.Email);

            if (checkingEmail != null)
                return (false, "Email already in use");

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if(!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            var roleExist = await _userManager.IsInRoleAsync(user, dto.Role);

            if (!roleExist)
                await _userManager.AddToRoleAsync(user, dto.Role);

            return (true, "User registered successfully");
        }

        //Login

        public async Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return (false, "Invalid login attempt", null);

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, dto.Password, false);

            if (!result.Succeeded)
                return (false, "Invalid login attempt", null);

            var token = await GenerateJwtToken(user);

            return (true, "Login successful", token);
        }



    }
}
