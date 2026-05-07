namespace TaxiBlazorClient.Models
{
    public class MonthlyTaxiReport
    {
        public decimal TotalRevenue { get; set; }
        public decimal BossRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal EmployeeCommission { get; set; }
        public decimal NetProfit { get; set; }
    }
}

