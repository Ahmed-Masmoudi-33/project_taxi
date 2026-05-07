namespace taxi.DTO.Expenses
{
    public class ExpenseResponseDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
    }
}

