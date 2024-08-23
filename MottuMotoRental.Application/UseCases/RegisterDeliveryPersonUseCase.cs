using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using MottuMotoRental.Application.DTOs;

namespace MottuMotoRental.Application.UseCases
{
    public class RegisterDeliveryPersonUseCase
    {
        private readonly IDeliveryPersonRepository _deliveryPersonRepository;
        private readonly ILogger<RegisterDeliveryPersonUseCase> _logger;
        private readonly IFileStorageService _fileStorageService;

        public RegisterDeliveryPersonUseCase(IDeliveryPersonRepository deliveryPersonRepository,
                                             ILogger<RegisterDeliveryPersonUseCase> logger,
                                             IFileStorageService fileStorageService)
        {
            _deliveryPersonRepository = deliveryPersonRepository;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        public async Task ExecuteAsync(RegisterDeliveryPersonDto dto)
        {
            _logger.LogInformation("Attempting to register a new delivery person with CNPJ {Cnpj}", dto.Cnpj);

            
            if (!IsValidCnpj(dto.Cnpj))
            {
                var errorMessage = $"Invalid CNPJ format for '{dto.Cnpj}'.";
                _logger.LogWarning(errorMessage);
                throw new InvalidCnpjFormatException(dto.Cnpj);
            }

            
            if (!IsValidDriverLicenseType(dto.DriverLicenseType))
            {
                var errorMessage = $"Invalid Driver License Type '{dto.DriverLicenseType}' for CNPJ '{dto.Cnpj}'.";
                _logger.LogWarning(errorMessage);
                throw new InvalidDriverLicenseTypeException(dto.DriverLicenseType);
            }

            var existingDeliveryPerson = await _deliveryPersonRepository.GetByCnpjAsync(dto.Cnpj);
            if (existingDeliveryPerson != null)
            {
                var errorMessage = $"Failed to register delivery person. CNPJ '{dto.Cnpj}' is already in use.";
                _logger.LogWarning(errorMessage);
                throw new CnpjAlreadyExistsException(dto.Cnpj);
            }

            existingDeliveryPerson = await _deliveryPersonRepository.GetByDriverLicenseNumberAsync(dto.DriverLicenseNumber);
            if (existingDeliveryPerson != null)
            {
                var errorMessage = $"Failed to register delivery person. Driver License Number '{dto.DriverLicenseNumber}' is already in use.";
                _logger.LogWarning(errorMessage);
                throw new DriverLicenseAlreadyExistsException(dto.DriverLicenseNumber);
            }

            
            var allowedFormats = new[] { ".png", ".bmp" };
            var fileExtension = System.IO.Path.GetExtension(dto.CnhImage.FileName).ToLower();

            if (!Array.Exists(allowedFormats, format => format == fileExtension))
            {
                var errorMessage = $"Invalid CNH image format '{fileExtension}' for CNPJ '{dto.Cnpj}'.";
                _logger.LogWarning(errorMessage);
                throw new InvalidCnhImageFormatException(fileExtension);
            }

            _logger.LogInformation("Uploading CNH image for delivery person with CNPJ {Cnpj}", dto.Cnpj);
            var cnhImageUrl = await _fileStorageService.UploadFileAsync(dto.CnhImage, dto.CnhImageFileName);

            var deliveryPerson = new DeliveryPerson
            {
                Identifier = dto.Identifier,
                Name = dto.Name,
                Cnpj = dto.Cnpj,
                BirthDate = dto.BirthDate,
                DriverLicenseNumber = dto.DriverLicenseNumber,
                DriverLicenseType = dto.DriverLicenseType,
                CnhImageUrl = cnhImageUrl
            };

            await _deliveryPersonRepository.AddAsync(deliveryPerson);

            _logger.LogInformation("Successfully registered delivery person with CNPJ {Cnpj}", dto.Cnpj);
        }

        private bool IsValidCnpj(string cnpj)
        {
            
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            
            if (cnpj.Length != 14)
                return false;

            
            if (new string(cnpj[0], cnpj.Length) == cnpj)
                return false;

            
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        private bool IsValidDriverLicenseType(string driverLicenseType)
        {
            var validTypes = new[] { "A", "B", "A+B" };
            return validTypes.Contains(driverLicenseType);
        }
    }
}
