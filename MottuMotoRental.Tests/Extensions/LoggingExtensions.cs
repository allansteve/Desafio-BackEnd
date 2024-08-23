using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace MottuMotoRental.Tests.Extensions
{
    public static class LoggingExtensions
    {
        public static void VerifyLog<T>(
            this Mock<ILogger<T>> mockLogger, 
            LogLevel logLevel, 
            string message, 
            Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                times);
        }



    }
}