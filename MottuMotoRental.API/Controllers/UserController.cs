using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MottuMotoRental.API.DTOs;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Application.UseCases.User;
using MottuMotoRental.Infrastructure.Data.Util;

namespace MottuMotoRental.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = SystemRolesConst.SystemAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]    
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ListUserUseCase _listUserUseCase;
        public UserController(ListUserUseCase listUserUseCase)
        {
            _listUserUseCase = listUserUseCase;
        }

        [HttpGet("list-users")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _listUserUseCase.Execute();
            return Ok(users);
        }
    }
}
