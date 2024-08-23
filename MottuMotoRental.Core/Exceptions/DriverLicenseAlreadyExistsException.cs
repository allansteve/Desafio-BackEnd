using System;

namespace MottuMotoRental.Core.Exceptions
{
    public class DriverLicenseAlreadyExistsException : Exception
    {
        public DriverLicenseAlreadyExistsException(string driverLicenseNumber)
            : base($"A delivery person with the driver license number '{driverLicenseNumber}' already exists.")
        {
        }
    }
}