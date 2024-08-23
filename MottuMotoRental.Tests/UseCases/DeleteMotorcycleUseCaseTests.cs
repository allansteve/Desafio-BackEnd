using Xunit;
using Moq;
using FluentAssertions;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class DeleteMotorcycleUseCaseTests
{
    [Fact]
    public async Task Should_DeleteMotorcycle_And_PublishEvent_When_NoActiveRentals()
    {
       
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<DeleteMotorcycleUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();
        
        var useCase = new DeleteMotorcycleUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        var motorcycle = new Motorcycle { Id = 1 };
        mockRepository.Setup(repo => repo.GetByIdAsync(motorcycle.Id)).ReturnsAsync(motorcycle);
        mockRepository.Setup(repo => repo.HasActiveRentalsAsync(motorcycle.Id)).ReturnsAsync(false);

       
        await useCase.ExecuteAsync(motorcycle.Id);

       
        mockRepository.Verify(repo => repo.DeleteAsync(motorcycle.Id), Times.Once);
        mockEventPublisher.Verify(ep => ep.Publish(It.Is<object>(o => o.ToString().Contains("MotorcycleDeleted"))), Times.Once);
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_MotorcycleDoesNotExist()
    {
        
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<DeleteMotorcycleUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new DeleteMotorcycleUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Motorcycle)null);

        
        Func<Task> act = async () => await useCase.ExecuteAsync(1);

        
        await act.Should().ThrowAsync<EntityNotFoundException>();
        mockEventPublisher.Verify(ep => ep.Publish(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task Should_ThrowInvalidOperationException_When_MotorcycleHasActiveRentals()
    {
       
        var mockRepository = new Mock<IMotorcycleRepository>();
        var mockLogger = new Mock<ILogger<DeleteMotorcycleUseCase>>();
        var mockEventPublisher = new Mock<IEventPublisher>();

        var useCase = new DeleteMotorcycleUseCase(mockRepository.Object, mockLogger.Object, mockEventPublisher.Object);

        var motorcycle = new Motorcycle { Id = 1 };
        mockRepository.Setup(repo => repo.GetByIdAsync(motorcycle.Id)).ReturnsAsync(motorcycle);
        mockRepository.Setup(repo => repo.HasActiveRentalsAsync(motorcycle.Id)).ReturnsAsync(true);

        
        Func<Task> act = async () => await useCase.ExecuteAsync(motorcycle.Id);

      
        await act.Should().ThrowAsync<InvalidOperationException>();
        mockEventPublisher.Verify(ep => ep.Publish(It.IsAny<object>()), Times.Never);
    }
}
