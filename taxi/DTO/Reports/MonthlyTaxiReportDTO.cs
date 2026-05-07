namespace taxi.DTO.Reports
{
    public class MonthlyTaxiReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal BossRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal EmployeeCommission { get; set; }
        public decimal NetProfit { get; set; }
    }
}
