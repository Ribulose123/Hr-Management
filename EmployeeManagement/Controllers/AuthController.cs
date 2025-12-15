using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationServices _authService;
        public AuthController(IAuthenticationServices authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, message) = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return BadRequest(new { Message = message });
            }
            return Ok(new { Message = message });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(new
            {
                token = result.Token
            });
        }
    }
}
