using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MottuMotoRental.Application.UseCases
{
    public class DeleteMotorcycleUseCase
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly ILogger<DeleteMotorcycleUseCase> _logger;
        private readonly IEventPublisher _eventPublisher;

        public DeleteMotorcycleUseCase(IMotorcycleRepository motorcycleRepository, ILogger<DeleteMotorcycleUseCase> logger, IEventPublisher eventPublisher)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task ExecuteAsync(int motorcycleId)
        {
            _logger.LogInformation("Attempting to delete motorcycle with ID {MotorcycleId}.", motorcycleId);

            var motorcycle = await _motorcycleRepository.GetByIdAsync(motorcycleId);
            if (motorcycle == null)
            {
                var errorMessage = $"Motorcycle with ID {motorcycleId} not found.";
                _logger.LogWarning(errorMessage);
                throw new EntityNotFoundException(errorMessage);
            }

            _logger.LogInformation("Checking if motorcycle with ID {MotorcycleId} has active rentals.", motorcycleId);
            if (await _motorcycleRepository.HasActiveRentalsAsync(motorcycleId))
            {
                var errorMessage = $"Cannot delete motorcycle with ID {motorcycleId} because it has active rentals.";
                _logger.LogWarning(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            await _motorcycleRepository.DeleteAsync(motorcycleId);
            _logger.LogInformation("Successfully deleted motorcycle with ID {MotorcycleId}.", motorcycleId);
            
            _eventPublisher.Publish(new { Event = "MotorcycleDeleted", Data = new { MotorcycleId = motorcycleId } });
        }
    }
}
