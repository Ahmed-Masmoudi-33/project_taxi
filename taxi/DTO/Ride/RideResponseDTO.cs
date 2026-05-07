namespace taxi.DTO.Ride
{
    public class RideResponseDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DistanceKm { get; set; }
        public decimal Amount { get; set; }
        public int TaxiId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? TaxiPlateNumber { get; set; }
        public string? TaxiGovernorate { get; set; }
    }
}

