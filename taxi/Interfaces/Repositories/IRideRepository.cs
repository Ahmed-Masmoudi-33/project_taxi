using metiers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace taxi.Interfaces.Repositories
{
    public interface IRideRepository
    {
        Task AddAsync(Ride ride);
        Task<List<Ride>> GetByTaxiAsync(int taxiId, DateTime start, DateTime end);
        Task<List<Ride>> GetByEmployeeAsync(int employeeId, DateTime start, DateTime end);
        Task<List<Ride>> GetAllByEmployeeAsync(int employeeId);
        Task<List<Ride>> GetByEmployeeAndTaxiAsync(int employeeId, int? taxiId);
        Task<List<Ride>> GetByBossTaxisAsync(int bossId, int? taxiId);
    }
}
