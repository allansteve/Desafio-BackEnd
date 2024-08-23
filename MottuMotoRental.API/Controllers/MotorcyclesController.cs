using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Data.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MottuMotoRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = SystemRolesConst.SystemAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MotorcyclesController : ControllerBase
    {
        private readonly RegisterMotorcycleUseCase _registerMotorcycleUseCase;
        private readonly GetMotorcyclesUseCase _getMotorcyclesUseCase;
        private readonly UpdateMotorcycleLicensePlateUseCase _updateMotorcycleLicensePlateUseCase;
        private readonly DeleteMotorcycleUseCase _deleteMotorcycleUseCase;

        public MotorcyclesController(
            RegisterMotorcycleUseCase registerMotorcycleUseCase,
            GetMotorcyclesUseCase getMotorcyclesUseCase,
            UpdateMotorcycleLicensePlateUseCase updateMotorcycleLicensePlateUseCase,
            DeleteMotorcycleUseCase deleteMotorcycleUseCase)
        {
            _registerMotorcycleUseCase = registerMotorcycleUseCase;
            _getMotorcyclesUseCase = getMotorcyclesUseCase;
            _updateMotorcycleLicensePlateUseCase = updateMotorcycleLicensePlateUseCase;
            _deleteMotorcycleUseCase = deleteMotorcycleUseCase;
        }

        
        [HttpPost("register-motorcycle")]
        public async Task<IActionResult> RegisterMotorcycle([FromBody] Motorcycle motorcycle)
        {
            await _registerMotorcycleUseCase.ExecuteAsync(motorcycle);
            return CreatedAtAction(nameof(RegisterMotorcycle), new { id = motorcycle.Id }, motorcycle);
        }
        
        [HttpGet("get-motorcycles")]
        public async Task<ActionResult<IEnumerable<Motorcycle>>> GetMotorcyclesWithPlate([FromQuery] string licensePlateFilter = null)
        {
            var motorcycles = await _getMotorcyclesUseCase.ExecuteAsync(licensePlateFilter);

            if (motorcycles == null || !motorcycles.Any())
            {
                return NotFound();
            }

            return Ok(motorcycles);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMotorcycleLicensePlate(int id, [FromBody] string newLicensePlate)
        {
            await _updateMotorcycleLicensePlateUseCase.ExecuteAsync(id, newLicensePlate);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotorcycle(int id)
        {
            await _deleteMotorcycleUseCase.ExecuteAsync(id);
            return NoContent();
        }
    }
}
