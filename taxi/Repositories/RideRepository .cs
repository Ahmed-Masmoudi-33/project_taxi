using metiers;
using taxi.Data;
using taxi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace taxi.Repositories
{
    public class RideRepository : IRideRepository
    {
        private readonly ApplicationContext _context;

        public RideRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Ride ride)
        {
            _context.Rides.Add(ride);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Ride>> GetByTaxiAsync(int taxiId, DateTime start, DateTime end)
        {
            return await _context.Rides
                .Where(r => r.TaxiId == taxiId &&
                            r.StartDate >= start &&
                            r.EndDate <= end)
                .ToListAsync();
        }

        public async Task<List<Ride>> GetByEmployeeAsync(int employeeId, DateTime start, DateTime end)
        {
            return await _context.Rides
                .Where(r => r.EmployeeId == employeeId &&
                            r.StartDate >= start &&
                            r.EndDate <= end)
                .Include(r => r.Taxi)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<List<Ride>> GetAllByEmployeeAsync(int employeeId)
        {
            return await _context.Rides
                .Where(r => r.EmployeeId == employeeId)
                .Include(r => r.Taxi)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<List<Ride>> GetByEmployeeAndTaxiAsync(int employeeId, int? taxiId)
        {
            IQueryable<Ride> query = _context.Rides
                .Where(r => r.EmployeeId == employeeId);

            if (taxiId.HasValue)
            {
                query = query.Where(r => r.TaxiId == taxiId.Value);
            }

            return await query
                .Include(r => r.Taxi)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<List<Ride>> GetByBossTaxisAsync(int bossId, int? taxiId)
        {
            // First get all taxi IDs that belong to this boss
            var bossTaxiIds = await _context.Taxis
                .Where(t => t.BossId == bossId)
                .Select(t => t.Id)
                .ToListAsync();

            if (!bossTaxiIds.Any())
            {
                return new List<Ride>();
            }

            IQueryable<Ride> query = _context.Rides
                .Where(r => bossTaxiIds.Contains(r.TaxiId));

            if (taxiId.HasValue)
            {
                // Verify the taxi belongs to the boss
                if (bossTaxiIds.Contains(taxiId.Value))
                {
                    query = query.Where(r => r.TaxiId == taxiId.Value);
                }
                else
                {
                    // Taxi doesn't belong to boss, return empty list
                    return new List<Ride>();
                }
            }

            return await query
                .Include(r => r.Taxi)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }
    }
}
