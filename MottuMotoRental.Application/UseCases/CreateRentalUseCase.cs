using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Enums;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MottuMotoRental.Application.UseCases
{
    public class CreateRentalUseCase
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IDeliveryPersonRepository _deliveryPersonRepository;
        private readonly ILogger<CreateRentalUseCase> _logger;

        public CreateRentalUseCase(
            IRentalRepository rentalRepository,
            IMotorcycleRepository motorcycleRepository,
            IDeliveryPersonRepository deliveryPersonRepository,
            ILogger<CreateRentalUseCase> logger)
        {
            _rentalRepository = rentalRepository;
            _motorcycleRepository = motorcycleRepository;
            _deliveryPersonRepository = deliveryPersonRepository;
            _logger = logger;
        }

        public async Task<Rental> ExecuteAsync(int deliveryPersonId, int motorcycleId, RentalPlan plan)
        {
            _logger.LogInformation("Attempting to create a new rental for delivery person with ID {DeliveryPersonId} and motorcycle ID {MotorcycleId}.", deliveryPersonId, motorcycleId);

            
            var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(deliveryPersonId);
            if (deliveryPerson == null || deliveryPerson.DriverLicenseType != "A")
            {
                var errorMessage = $"The delivery person with ID {deliveryPersonId} is not qualified to rent a motorcycle.";
                _logger.LogWarning(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

           
            var motorcycle = await _motorcycleRepository.GetByIdAsync(motorcycleId);
            if (motorcycle == null || !motorcycle.IsActive)
            {
                var errorMessage = $"The motorcycle with ID {motorcycleId} is not available for rental.";
                _logger.LogWarning(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

       
            var rental = new Rental
            {
                DeliveryPersonId = deliveryPersonId,
                MotorcycleId = motorcycleId,
                Plan = plan,
                StartDate = DateTime.UtcNow,
                ExpectedEndDate = DateTime.UtcNow.AddDays((int)plan),
                EndDate = DateTime.MinValue,
                CostPerDay = GetCostPerDay(plan),
                IsActive = true
            };

            await _rentalRepository.AddAsync(rental);

            _logger.LogInformation("Rental created successfully for delivery person ID {DeliveryPersonId} with motorcycle ID {MotorcycleId}.", deliveryPersonId, motorcycleId);

            return rental;
        }

        private decimal GetCostPerDay(RentalPlan plan)
        {
            return plan switch
            {
                RentalPlan.SevenDays => 30m,
                RentalPlan.FifteenDays => 28m,
                RentalPlan.ThirtyDays => 22m,
                RentalPlan.FortyFiveDays => 20m,
                RentalPlan.SixtyDays => 18m,
                _ => throw new ArgumentOutOfRangeException(nameof(plan), plan, null)
            };
        }
    }
}
