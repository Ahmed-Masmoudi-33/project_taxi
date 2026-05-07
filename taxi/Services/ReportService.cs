using taxi.DTO.Reports;
using taxi.Interfaces.Repositories;
using taxi.Data;
using Microsoft.EntityFrameworkCore;
using Enums;

namespace taxi.Services
{
    public class ReportService
    {
        private readonly IRideRepository _rideRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly ICommissionRepository _commissionRepo;
        private readonly ApplicationContext _context;

        public ReportService(
            IRideRepository rideRepo,
            IExpenseRepository expenseRepo,
            ICommissionRepository commissionRepo,
            ApplicationContext context)
        {
            _rideRepo = rideRepo;
            _expenseRepo = expenseRepo;
            _commissionRepo = commissionRepo;
            _context = context;
        }

        public async Task<MonthlyTaxiReportDto> GenerateAsync(
            int taxiId, int bossId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            // Get rides with employee information
            var rides = await _context.Rides
                .Include(r => r.Employee)
                .Where(r => r.TaxiId == taxiId &&
                           r.StartDate >= start &&
                           r.EndDate <= end)
                .ToListAsync();

            var expenses = await _expenseRepo.GetByTaxiAsync(taxiId, start, end);
            var commission = await _commissionRepo.GetByBossAsync(bossId);
            var commissionPercentage = commission?.Percentage ?? 0;

            decimal totalRevenue = 0;
            decimal bossRevenue = 0;
            decimal employeeCommission = 0;

            // Calculate revenue split based on who did the ride
            foreach (var ride in rides)
            {
                totalRevenue += ride.Amount;

                // If the ride was done by the boss (EmployeeId == BossId), boss gets 100%
                if (ride.EmployeeId == bossId)
                {
                    bossRevenue += ride.Amount;
                }
                else
                {
                    // If done by an employee:
                    // Employee gets commission percentage
                    // Boss gets the rest (100% - commission%)
                    var employeeAmount = ride.Amount * (commissionPercentage / 100);
                    var bossAmount = ride.Amount - employeeAmount;

                    employeeCommission += employeeAmount;
                    bossRevenue += bossAmount;
                }
            }

            var expenseTotal = expenses.Sum(e => e.Amount);
            var netProfit = bossRevenue - expenseTotal;

            return new MonthlyTaxiReportDto
            {
                TotalRevenue = totalRevenue,
                BossRevenue = bossRevenue,
                TotalExpenses = expenseTotal,
                EmployeeCommission = employeeCommission,
                NetProfit = netProfit
            };
        }
    }
}
