namespace taxi.DTO.Taxi
{
    public class TaxiResponseDTO
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string? AssignedEmployeeName { get; set; }
        public string? AssignedEmployeeCIN { get; set; }

    }
}
