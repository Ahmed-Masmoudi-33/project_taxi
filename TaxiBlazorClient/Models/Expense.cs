namespace TaxiBlazorClient.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }
        public int TaxiId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
    }

    public class ExpenseCreateRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Type { get; set; } = "OTHER";
        public string? Description { get; set; }
    }
}

