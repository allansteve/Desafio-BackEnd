using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MottuMotoRental.Application.DTOs;
using MottuMotoRental.Application.UseCases.User;

namespace MottuMotoRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginUserUseCase _loginUserUseCase;
        public AuthController(LoginUserUseCase loginUserUseCase)
        {
            _loginUserUseCase = loginUserUseCase;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _loginUserUseCase.Execute(loginDto);

            return user.Result switch
            {
                LoginResult.Success => Ok(new {Token=user.Token }),
                LoginResult.InvalidCredentials => Unauthorized(new { error = "Invalid credentials" }),
                LoginResult.UserNotFound => NotFound(new { error = "User not found" }),
                _ => StatusCode(500, new { error = "An unexpected error occurred." })
            };  
        }

    }
}
