using Xunit;
using Moq;
using FluentAssertions;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Exceptions;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MottuMotoRental.Core.Enums;
using MottuMotoRental.Tests.Extensions;

public class FinalizeRentalUseCaseTests
{
    [Fact]
    public async Task Should_FinalizeRental_With_CorrectTotalCost_When_RentalReturnedOnTime()
    {
        
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockLogger = new Mock<ILogger<FinalizeRentalUseCase>>();
        var useCase = new FinalizeRentalUseCase(mockRentalRepository.Object, mockLogger.Object);

        var rental = new Rental
        {
            Id = 1,
            StartDate = DateTime.UtcNow.AddDays(-7),
            ExpectedEndDate = DateTime.UtcNow,
            EndDate = DateTime.MinValue,
            CostPerDay = 30m,
            Plan = RentalPlan.SevenDays
        };

        mockRentalRepository.Setup(repo => repo.GetByIdAsync(rental.Id)).ReturnsAsync(rental);

        var returnDate = DateTime.UtcNow;

        
        var result = await useCase.ExecuteAsync(rental.Id, returnDate);

        
        result.EndDate.Should().Be(returnDate);
        result.TotalCost.Should().Be(210m); 
        mockRentalRepository.Verify(repo => repo.UpdateAsync(rental), Times.Once);
        mockLogger.VerifyLog(LogLevel.Information, $"Rental with ID {rental.Id} finalized successfully. Total cost: {result.TotalCost}.", Times.Once());
    }

    [Fact]
    public async Task Should_ApplyPenalty_When_RentalReturnedEarlier()
    {
       
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockLogger = new Mock<ILogger<FinalizeRentalUseCase>>();
        var useCase = new FinalizeRentalUseCase(mockRentalRepository.Object, mockLogger.Object);

        var rental = new Rental
        {
            Id = 1,
            StartDate = DateTime.UtcNow.AddDays(-7),
            ExpectedEndDate = DateTime.UtcNow.AddDays(2), 
            EndDate = DateTime.MinValue,
            CostPerDay = 30m,
            Plan = RentalPlan.SevenDays
        };

        mockRentalRepository.Setup(repo => repo.GetByIdAsync(rental.Id)).ReturnsAsync(rental);

        var returnDate = DateTime.UtcNow; 

        
        var result = await useCase.ExecuteAsync(rental.Id, returnDate);

        
        var daysUsed = (returnDate - rental.StartDate).Days; 
        var penaltyDays = (rental.ExpectedEndDate - returnDate).Days; 
        var expectedPenalty = penaltyDays * rental.CostPerDay * 0.20m; 
        var expectedTotalCost = (daysUsed * rental.CostPerDay) + expectedPenalty; 

       
        result.EndDate.Should().Be(returnDate);
        result.TotalCost.Should().Be(expectedTotalCost); 

        mockRentalRepository.Verify(repo => repo.UpdateAsync(rental), Times.Once);

        mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Rental with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }



    [Fact]
    public async Task Should_AddExtraCost_When_RentalReturnedLater()
    {
      
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockLogger = new Mock<ILogger<FinalizeRentalUseCase>>();
        var useCase = new FinalizeRentalUseCase(mockRentalRepository.Object, mockLogger.Object);

        var rental = new Rental
        {
            Id = 1,
            StartDate = DateTime.UtcNow.AddDays(-7),
            ExpectedEndDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.MinValue,
            CostPerDay = 30m,
            Plan = RentalPlan.SevenDays
        };

        mockRentalRepository.Setup(repo => repo.GetByIdAsync(rental.Id)).ReturnsAsync(rental);

        var returnDate = DateTime.UtcNow;

        
        var result = await useCase.ExecuteAsync(rental.Id, returnDate);

       
        result.EndDate.Should().Be(returnDate);
        
        result.TotalCost.Should().Be(210m + 100m);
        mockRentalRepository.Verify(repo => repo.UpdateAsync(rental), Times.Once);
        mockLogger.VerifyLog(LogLevel.Information, $"Rental with ID {rental.Id} finalized successfully. Total cost: {result.TotalCost}.", Times.Once());
    }

    [Fact]
    public async Task Should_ThrowException_When_RentalNotFound()
    {
      
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockLogger = new Mock<ILogger<FinalizeRentalUseCase>>();
        var useCase = new FinalizeRentalUseCase(mockRentalRepository.Object, mockLogger.Object);

        mockRentalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Rental)null);

        
        Func<Task> act = async () => await useCase.ExecuteAsync(1, DateTime.UtcNow);

        
        await act.Should().ThrowAsync<EntityNotFoundException>();
        mockLogger.VerifyLog(LogLevel.Warning, "Rental with ID 1 not found.", Times.Once());
    }

    [Fact]
    public async Task Should_ThrowException_When_RentalAlreadyFinalized()
    {
     
        var mockRentalRepository = new Mock<IRentalRepository>();
        var mockLogger = new Mock<ILogger<FinalizeRentalUseCase>>();
        var useCase = new FinalizeRentalUseCase(mockRentalRepository.Object, mockLogger.Object);

        var rental = new Rental
        {
            Id = 1,
            StartDate = DateTime.UtcNow.AddDays(-7),
            ExpectedEndDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            CostPerDay = 30m,
            Plan = RentalPlan.SevenDays
        };

        mockRentalRepository.Setup(repo => repo.GetByIdAsync(rental.Id)).ReturnsAsync(rental);

       
        Func<Task> act = async () => await useCase.ExecuteAsync(rental.Id, DateTime.UtcNow);

        
        await act.Should().ThrowAsync<InvalidOperationException>();
        mockLogger.VerifyLog(LogLevel.Warning, "Rental with ID 1 has already been finalized.", Times.Once());
    }
}
