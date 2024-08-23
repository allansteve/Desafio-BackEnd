using Xunit;
using Moq;
using FluentAssertions;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using MottuMotoRental.Application.DTOs;

public class RegisterDeliveryPersonUseCaseTests
{
    [Fact]
    public async Task Should_ThrowInvalidCnpjFormatException_When_CnpjIsInvalid()
    {
        
        var mockRepository = new Mock<IDeliveryPersonRepository>();
        var mockLogger = new Mock<ILogger<RegisterDeliveryPersonUseCase>>();
        var mockStorageService = new Mock<IFileStorageService>();

        var useCase = new RegisterDeliveryPersonUseCase(mockRepository.Object, mockLogger.Object, mockStorageService.Object);

        var dto = new RegisterDeliveryPersonDto
        {
            Cnpj = "1234567890", 
            DriverLicenseNumber = "123456789",
            CnhImage = new MockFormFile(new MemoryStream(), "test.png") 
        };

        
        Func<Task> act = async () => await useCase.ExecuteAsync(dto);

        
        await act.Should().ThrowAsync<InvalidCnpjFormatException>();
    }
}
