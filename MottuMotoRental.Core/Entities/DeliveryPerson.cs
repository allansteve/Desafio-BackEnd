namespace MottuMotoRental.Core.Entities
{
    public class DeliveryPerson
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public DateTime BirthDate { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseType { get; set; }
        public string CnhImageUrl { get; set; }
    }
}