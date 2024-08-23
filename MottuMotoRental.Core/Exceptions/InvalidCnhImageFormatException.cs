using System;

namespace MottuMotoRental.Core.Exceptions
{
    public class InvalidCnhImageFormatException : Exception
    {
        public InvalidCnhImageFormatException(string format)
            : base($"The CNH image format '{format}' is not supported. Allowed formats: .png, .bmp.")
        {
        }
    }
}