using MottuMotoRental.Core.Enums;

public class Rental
{
    public int Id { get; set; }
    public int DeliveryPersonId { get; set; }
    public int MotorcycleId { get; set; }
    public RentalPlan Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal CostPerDay { get; set; }
    public decimal TotalCost { get; set; }
    public bool IsActive { get; set; }
}