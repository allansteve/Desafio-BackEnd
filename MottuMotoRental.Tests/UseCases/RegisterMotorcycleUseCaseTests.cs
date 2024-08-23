using Xunit;
using Moq;
using FluentAssertions;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Application.DTOs;
using MottuMotoRental.Core.Exceptions;
using Microsoft.Extensions.Logging;
using MottuMotoRental.Infrastructure.Messaging;
using System.Threading.Tasks;

public class RegisterMotorcycleUseCaseTests
{
    [Fact]
    public async Task Should_RegisterMotorcycle_And_PublishEvent_When_ValidDataProvided()
    {
        
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<RegisterMotorcycleUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new RegisterMotorcycleUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        
        var motorcycle = new Motorcycle
        {
            Identifier = "123ABC",
            Year = 2023,
            Model = "Model X",
            LicensePlate = "XYZ-9876"
        };

        
        var result = await useCase.ExecuteAsync(motorcycle);

        
        result.Identifier.Should().Be(motorcycle.Identifier);
        result.LicensePlate.Should().Be(motorcycle.LicensePlate);
        mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Motorcycle>()), Times.Once);
        mockEventPublisher.Verify(ep => ep.Publish(It.Is<object>(o => o.ToString().Contains("MotorcycleRegistered"))), Times.Once);
    }   

    [Fact]
    public async Task Should_ThrowLicensePlateAlreadyExistsException_And_NotPublishEvent_When_LicensePlateAlreadyExists()
    {
        
        var mockRepository = new Mock<IMotorcycleRepository>();
        mockRepository.Setup(repo => repo.GetByLicensePlateAsync(It.IsAny<string>()))
                      .ReturnsAsync(new Motorcycle());

        var mockLogger = new Mock<ILogger<RegisterMotorcycleUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new RegisterMotorcycleUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);


        var licensePlate = "XYZ-9876";
        
        var motorcycle = new Motorcycle
        {
            Identifier = "123ABC",
            Year = 2023,
            Model = "Model X",
            LicensePlate = licensePlate
        };

      
        Func<Task> act = async () => await useCase.ExecuteAsync(motorcycle);

      
        await act.Should().ThrowAsync<LicensePlateAlreadyExistsException>()
                  .WithMessage($"A motorcycle with the license plate '{licensePlate}' already exists.");
        mockEventPublisher.Verify(ep => ep.Publish(It.IsAny<object>()), Times.Never);
    }
}
