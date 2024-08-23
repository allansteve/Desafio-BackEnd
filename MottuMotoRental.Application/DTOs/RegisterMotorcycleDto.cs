namespace MottuMotoRental.Application.DTOs
{
    public class RegisterMotorcycleDto
    {
        public string Identifier { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
    }

    public class MotorcycleDto
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
    }
}