using System;

namespace MottuMotoRental.Core.Exceptions
{
    public class CnpjAlreadyExistsException : Exception
    {
        public CnpjAlreadyExistsException(string cnpj)
            : base($"A delivery person with the CNPJ '{cnpj}' already exists.")
        {
        }
    }
}