using System;

namespace MottuMotoRental.Core.Exceptions
{
    public class LicensePlateAlreadyExistsException : Exception
    {
        public LicensePlateAlreadyExistsException() 
            : base("A motorcycle with the same license plate already exists.")
        {
        }

        public LicensePlateAlreadyExistsException(string licensePlate) 
            : base($"A motorcycle with the license plate '{licensePlate}' already exists.")
        {
        }

        public LicensePlateAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}