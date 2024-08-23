using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MottuMotoRental.Application.UseCases
{
    public class GetMotorcyclesUseCase
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly ILogger<GetMotorcyclesUseCase> _logger;

        public GetMotorcyclesUseCase(IMotorcycleRepository motorcycleRepository, ILogger<GetMotorcyclesUseCase> logger)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<MotorcycleDto>> ExecuteAsync(string licensePlateFilter = null)
        {
            _logger.LogInformation("Fetching motorcycles from the repository.");

            var motorcycles = await _motorcycleRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(licensePlateFilter))
            {
                _logger.LogInformation("Applying filter by license plate: {LicensePlateFilter}", licensePlateFilter);
                motorcycles = motorcycles.Where(m => m.LicensePlate.Contains(licensePlateFilter));
            }

            _logger.LogInformation("Returning {Count} motorcycles.", motorcycles.Count());

            return motorcycles.Select(m => new MotorcycleDto
            {
                Id = m.Id,
                Identifier = m.Identifier,
                Year = m.Year,
                Model = m.Model,
                LicensePlate = m.LicensePlate
            });
        }
    }
}