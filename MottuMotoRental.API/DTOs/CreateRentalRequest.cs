namespace MottuMotoRental.API.DTOs
{
    public class CreateRentalRequest
    {
        public int DeliveryPersonId { get; set; }
        public int MotorcycleId { get; set; }
        public Core.Enums.RentalPlan Plan { get; set; }
    }
}