using Microsoft.AspNetCore.Http;

namespace MottuMotoRental.Application.DTOs
{
    public class RegisterDeliveryPersonDto
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public DateTime BirthDate { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseType { get; set; }
        public IFormFile CnhImage { get; set; }
        public string CnhImageFileName => $"{Cnpj}_CNH.{CnhImage.FileName.Split('.').Last()}";
    }
}