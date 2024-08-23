using Xunit;
using Moq;
using FluentAssertions;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class UpdateMotorcycleLicensePlateUseCaseTests
{
    [Fact]
    public async Task Should_UpdateLicensePlate_And_PublishEvent_When_ValidDataProvided()
    {
        
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<UpdateMotorcycleLicensePlateUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new UpdateMotorcycleLicensePlateUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        var motorcycle = new Motorcycle { Id = 1, LicensePlate = "OLD123" };
        mockRepository.Setup(repo => repo.GetByIdAsync(motorcycle.Id)).ReturnsAsync(motorcycle);
        mockRepository.Setup(repo => repo.GetByLicensePlateAsync("NEW123")).ReturnsAsync((Motorcycle)null);

        
        await useCase.ExecuteAsync(motorcycle.Id, "NEW123");

        
        motorcycle.LicensePlate.Should().Be("NEW123");
        mockRepository.Verify(repo => repo.UpdateAsync(motorcycle), Times.Once);
        mockEventPublisher.Verify(ep => ep.Publish(It.Is<object>(o => o.ToString().Contains("MotorcycleLicensePlateUpdated"))), Times.Once);
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_And_NotPublishEvent_When_MotorcycleDoesNotExist()
    {
        
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<UpdateMotorcycleLicensePlateUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new UpdateMotorcycleLicensePlateUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Motorcycle)null);

        
        Func<Task> act = async () => await useCase.ExecuteAsync(1, "NEW123");

        
        await act.Should().ThrowAsync<EntityNotFoundException>();
        mockEventPublisher.Verify(ep => ep.Publish(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task Should_ThrowLicensePlateAlreadyExistsException_And_NotPublishEvent_When_LicensePlateIsTaken()
    {
        
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<UpdateMotorcycleLicensePlateUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new UpdateMotorcycleLicensePlateUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        var motorcycle = new Motorcycle { Id = 1, LicensePlate = "OLD123" };
        var anotherMotorcycle = new Motorcycle { Id = 2, LicensePlate = "NEW123" };

        mockRepository.Setup(repo => repo.GetByIdAsync(motorcycle.Id)).ReturnsAsync(motorcycle);
        mockRepository.Setup(repo => repo.GetByLicensePlateAsync("NEW123")).ReturnsAsync(anotherMotorcycle);

        
        Func<Task> act = async () => await useCase.ExecuteAsync(motorcycle.Id, "NEW123");

       
        await act.Should().ThrowAsync<LicensePlateAlreadyExistsException>();
        mockEventPublisher.Verify(ep => ep.Publish(It.IsAny<object>()), Times.Never);
    }
}
