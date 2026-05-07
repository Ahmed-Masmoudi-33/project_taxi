namespace TaxiBlazorClient.Models
{
    public class Taxi
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public int BossId { get; set; }
        public string? AssignedEmployeeName { get; set; }
        public string? AssignedEmployeeCIN { get; set; }
    }

    public class TaxiCreateRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string? EmployeeCIN { get; set; } // Optional: CIN of employee to assign
    }

    public class TaxiUpdateRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string? EmployeeCIN { get; set; } // Optional: CIN of employee to assign

    }
}

