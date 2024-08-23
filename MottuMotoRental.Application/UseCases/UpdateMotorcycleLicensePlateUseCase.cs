using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MottuMotoRental.Application.UseCases
{
    public class UpdateMotorcycleLicensePlateUseCase
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly ILogger<UpdateMotorcycleLicensePlateUseCase> _logger;
        private readonly IEventPublisher _eventPublisher;

        public UpdateMotorcycleLicensePlateUseCase(IMotorcycleRepository motorcycleRepository,
            ILogger<UpdateMotorcycleLicensePlateUseCase> logger, IEventPublisher eventPublisher)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task ExecuteAsync(int motorcycleId, string newLicensePlate)
        {
            
            _logger.LogInformation("Attempting to update license plate for motorcycle with ID {MotorcycleId}.",
                motorcycleId);

            
            var motorcycle = await _motorcycleRepository.GetByIdAsync(motorcycleId);
            if (motorcycle == null)
            {
                
                var errorMessage = $"Motorcycle with ID {motorcycleId} not found.";
                _logger.LogWarning(errorMessage);
                throw new EntityNotFoundException(errorMessage);
            }

          
            _logger.LogInformation("Checking if the new license plate {LicensePlate} is already in use.",
                newLicensePlate);
            var existingMotorcycle = await _motorcycleRepository.GetByLicensePlateAsync(newLicensePlate);
            if (existingMotorcycle != null && existingMotorcycle.Id != motorcycleId)
            {
                
                var errorMessage = $"License plate '{newLicensePlate}' is already in use by another motorcycle.";
                _logger.LogWarning(errorMessage);
                throw new LicensePlateAlreadyExistsException(newLicensePlate);
            }

            
            motorcycle.LicensePlate = newLicensePlate;
            await _motorcycleRepository.UpdateAsync(motorcycle);

           
            _logger.LogInformation(
                "Successfully updated license plate to {LicensePlate} for motorcycle with ID {MotorcycleId}.",
                newLicensePlate, motorcycleId);

            _eventPublisher.Publish(new
            {
                Event = "MotorcycleLicensePlateUpdated",
                Data = new { MotorcycleId = motorcycle.Id, NewLicensePlate = newLicensePlate }
            });
        }
    }
}
