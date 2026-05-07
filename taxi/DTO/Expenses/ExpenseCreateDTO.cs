using Enums;

namespace taxi.DTO.Expenses
{
    public class ExpenseCreateDTO
    {
        public string PlateNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Type { get; set; } = ExpenseType.OTHER;
        public string? Description { get; set; }
    }
}
