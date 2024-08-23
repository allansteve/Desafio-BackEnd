

namespace MottuMotoRental.Core.Entities
{
    public class Motorcycle
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public bool IsActive { get; set; }
    }
}