using metiers;

namespace taxi.Interfaces.Repositories
{
    public interface IAssignmentRepository
    {
        Task AddAsync(Assignment assignment);
        Task EndAssignmentAsync(int assignmentId, DateTime endDate);
        Task<List<Assignment>> GetByEmployeeAsync(int employeeId);
        Task<bool> IsEmployeeAssignedToTaxiAsync(int employeeId, int taxiId);
    }
}
