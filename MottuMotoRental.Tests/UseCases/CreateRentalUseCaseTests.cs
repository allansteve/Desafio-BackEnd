using Xunit;
using Moq;
using FluentAssertions;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Enums;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MottuMotoRental.Tests.Extensions;

public class CreateRentalUseCaseTests
{
    [Fact]
    public async Task Should_CreateRental_When_ValidDataProvided()
    {
        
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
        var mockDeliveryPersonRepository = new Mock<IDeliveryPersonRepository>();
        var mockLogger = new Mock<ILogger<CreateRentalUseCase>>();

        var useCase = new CreateRentalUseCase(
            mockRentalRepository.Object,
            mockMotorcycleRepository.Object,
            mockDeliveryPersonRepository.Object,
            mockLogger.Object);

        var deliveryPerson = new DeliveryPerson { Id = 1, DriverLicenseType = "A" };
        var motorcycle = new Motorcycle { Id = 1, IsActive = true };

        mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPerson.Id)).ReturnsAsync(deliveryPerson);
        mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(motorcycle.Id)).ReturnsAsync(motorcycle);

        
        var result = await useCase.ExecuteAsync(deliveryPerson.Id, motorcycle.Id, RentalPlan.SevenDays);

       
        result.Should().NotBeNull();
        result.DeliveryPersonId.Should().Be(deliveryPerson.Id);
        result.MotorcycleId.Should().Be(motorcycle.Id);
        result.Plan.Should().Be(RentalPlan.SevenDays);
        result.IsActive.Should().BeTrue();

        mockRentalRepository.Verify(repo => repo.AddAsync(It.IsAny<Rental>()), Times.Once);
        mockLogger.VerifyLog(LogLevel.Information, "Rental created successfully for delivery person ID 1 with motorcycle ID 1.", Times.AtLeastOnce());
    }

    [Fact]
    public async Task Should_ThrowInvalidOperationException_When_DeliveryPersonNotQualified()
    {
        
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
        var mockDeliveryPersonRepository = new Mock<IDeliveryPersonRepository>();
        var mockLogger = new Mock<ILogger<CreateRentalUseCase>>();

        var useCase = new CreateRentalUseCase(
            mockRentalRepository.Object,
            mockMotorcycleRepository.Object,
            mockDeliveryPersonRepository.Object,
            mockLogger.Object);

        mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((DeliveryPerson)null);

       
        Func<Task> act = async () => await useCase.ExecuteAsync(1, 1, RentalPlan.SevenDays);

        
        await act.Should().ThrowAsync<InvalidOperationException>();
        mockLogger.VerifyLog(LogLevel.Warning, "The delivery person with ID 1 is not qualified to rent a motorcycle.", Times.Once());
    }

    [Fact]
    public async Task Should_ThrowInvalidOperationException_When_MotorcycleNotAvailable()
    {
        
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
        var mockDeliveryPersonRepository = new Mock<IDeliveryPersonRepository>();
        var mockLogger = new Mock<ILogger<CreateRentalUseCase>>();

        var useCase = new CreateRentalUseCase(
            mockRentalRepository.Object,
            mockMotorcycleRepository.Object,
            mockDeliveryPersonRepository.Object,
            mockLogger.Object);

        var deliveryPerson = new DeliveryPerson { Id = 1, DriverLicenseType = "A" };
        mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPerson.Id)).ReturnsAsync(deliveryPerson);
        mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Motorcycle)null);

        
        Func<Task> act = async () => await useCase.ExecuteAsync(deliveryPerson.Id, 1, RentalPlan.SevenDays);

        
        await act.Should().ThrowAsync<InvalidOperationException>();
        mockLogger.VerifyLog(LogLevel.Warning, "The motorcycle with ID 1 is not available for rental.", Times.Once());
    }
}
