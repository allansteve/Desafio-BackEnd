using Microsoft.AspNetCore.Mvc;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Enums;
using System;
using System.Threading.Tasks;
using MottuMotoRental.API.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MottuMotoRental.Infrastructure.Data.Util;

namespace MottuMotoRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = SystemRolesConst.SystemDelivery, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RentalsController : ControllerBase
    {
        private readonly CreateRentalUseCase _createRentalUseCase;
        private readonly FinalizeRentalUseCase _finalizeRentalUseCase;

        public RentalsController(
            CreateRentalUseCase createRentalUseCase,
            FinalizeRentalUseCase finalizeRentalUseCase)
        {
            _createRentalUseCase = createRentalUseCase;
            _finalizeRentalUseCase = finalizeRentalUseCase;
        }

        [HttpPost("CreateRental")]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalRequest request)
        {
            var rental = await _createRentalUseCase.ExecuteAsync(request.DeliveryPersonId, request.MotorcycleId, request.Plan);
            return CreatedAtAction(nameof(CreateRental), new { id = rental.Id }, rental);
        }

        [HttpPost("{id}/finalize")]
        public async Task<IActionResult> FinalizeRental(int id, [FromBody] DateTime returnDate)
        {
            try
            {
                var rental = await _finalizeRentalUseCase.ExecuteAsync(id, returnDate);
                return Ok(rental);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}