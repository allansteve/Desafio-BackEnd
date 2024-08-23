using Microsoft.AspNetCore.Mvc;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MottuMotoRental.Infrastructure.Data.Util;

namespace MottuMotoRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = SystemRolesConst.SystemDelivery, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DeliveryPersonsController : ControllerBase
    {
        private readonly RegisterDeliveryPersonUseCase _registerDeliveryPersonUseCase;

        public DeliveryPersonsController(RegisterDeliveryPersonUseCase registerDeliveryPersonUseCase)
        {
            _registerDeliveryPersonUseCase = registerDeliveryPersonUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDeliveryPerson([FromBody] RegisterDeliveryPersonDto deliveryPersonDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _registerDeliveryPersonUseCase.ExecuteAsync(deliveryPersonDto);
            return CreatedAtAction(nameof(RegisterDeliveryPerson), new { id = deliveryPersonDto.Identifier }, deliveryPersonDto);
        }
    }
}