using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MottuMotoRental.Application.UseCases
{
    public class RegisterMotorcycleUseCase
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly ILogger<RegisterMotorcycleUseCase> _logger;
        private readonly IEventPublisher _eventPublisher;

        public RegisterMotorcycleUseCase(IMotorcycleRepository motorcycleRepository, ILogger<RegisterMotorcycleUseCase> logger, IEventPublisher eventPublisher)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<MotorcycleDto> ExecuteAsync(Motorcycle motorcycle)
        {
            _logger.LogInformation("Attempting to register a new motorcycle with license plate {LicensePlate}", motorcycle.LicensePlate);

            var existingMotorcycle = await _motorcycleRepository.GetByLicensePlateAsync(motorcycle.LicensePlate);
            if (existingMotorcycle != null)
            {
                var errorMessage = $"Failed to register motorcycle. License plate '{motorcycle.LicensePlate}' is already in use.";
        
                _logger.LogWarning(errorMessage);
        
                throw new LicensePlateAlreadyExistsException(motorcycle.LicensePlate);
            }

            await _motorcycleRepository.AddAsync(motorcycle);

            _logger.LogInformation("Successfully registered motorcycle with license plate {LicensePlate}", motorcycle.LicensePlate);
    
            _eventPublisher.Publish(new { Event = "MotorcycleRegistered", Data = motorcycle });
    
            return new MotorcycleDto
            {
                Id = motorcycle.Id,
                Identifier = motorcycle.Identifier,
                Year = motorcycle.Year,
                Model = motorcycle.Model,
                LicensePlate = motorcycle.LicensePlate
            };
        }
    }
}
