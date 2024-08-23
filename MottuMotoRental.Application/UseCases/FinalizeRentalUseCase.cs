using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MottuMotoRental.Core.Enums;

namespace MottuMotoRental.Application.UseCases
{
    public class FinalizeRentalUseCase
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ILogger<FinalizeRentalUseCase> _logger;

        public FinalizeRentalUseCase(IRentalRepository rentalRepository, ILogger<FinalizeRentalUseCase> logger)
        {
            _rentalRepository = rentalRepository;
            _logger = logger;
        }

        public async Task<Rental> ExecuteAsync(int rentalId, DateTime returnDate)
        {
            _logger.LogInformation("Attempting to finalize rental with ID {RentalId}.", rentalId);

            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null)
            {
                var errorMessage = $"Rental with ID {rentalId} not found.";
                _logger.LogWarning(errorMessage);
                throw new EntityNotFoundException(errorMessage);
            }

            if (rental.EndDate != DateTime.MinValue)
            {
                var errorMessage = $"Rental with ID {rentalId} has already been finalized.";
                _logger.LogWarning(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            rental.EndDate = returnDate;
            rental.TotalCost = CalculateTotalCost(rental, returnDate);

            await _rentalRepository.UpdateAsync(rental);

            _logger.LogInformation("Rental with ID {RentalId} finalized successfully. Total cost: {TotalCost}.",
                rentalId, rental.TotalCost);

            return rental;
        }

        private decimal CalculateTotalCost(Rental rental, DateTime returnDate)
        {
            decimal totalCost = rental.CostPerDay * (returnDate - rental.StartDate).Days;

            if (returnDate < rental.ExpectedEndDate)
            {
                var daysNotUsed = (rental.ExpectedEndDate - returnDate).Days;
                decimal penalty = rental.Plan switch
                {
                    RentalPlan.SevenDays => 0.20m,
                    RentalPlan.FifteenDays => 0.40m,
                    _ => 0m
                };
                totalCost += daysNotUsed * rental.CostPerDay * penalty;
            }
            else if (returnDate > rental.ExpectedEndDate)
            {
                var extraDays = (returnDate - rental.ExpectedEndDate).Days;
                totalCost += extraDays * 50m;
            }

            return totalCost;
        }
    }
}