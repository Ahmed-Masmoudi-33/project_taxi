namespace taxi.DTO.Ride
{
    public class RideCreateDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DistanceKm { get; set; }
        public decimal Amount { get; set; }
        public int TaxiId { get; set; }
    }
}
