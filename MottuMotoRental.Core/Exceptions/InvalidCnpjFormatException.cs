using System;

namespace MottuMotoRental.Core.Exceptions
{
    public class InvalidCnpjFormatException : Exception
    {
        public InvalidCnpjFormatException(string cnpj)
            : base($"The CNPJ '{cnpj}' is not in a valid format.")
        {
        }
    }
}