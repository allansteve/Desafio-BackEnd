using System;

namespace MottuMotoRental.Core.Exceptions
{
    public class InvalidDriverLicenseTypeException : Exception
    {
        public InvalidDriverLicenseTypeException(string driverLicenseType)
            : base($"Invalid driver license type: {driverLicenseType}.")
        {
        }
    }
}