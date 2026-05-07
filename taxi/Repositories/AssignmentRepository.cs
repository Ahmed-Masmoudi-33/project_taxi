using metiers;
using taxi.Interfaces.Repositories;
using taxi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace taxi.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationContext _context;

        public AssignmentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Assignment assignment)
        {
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task EndAssignmentAsync(int assignmentId, DateTime endDate)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null) return;

            assignment.EndDate = endDate;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Assignment>> GetByEmployeeAsync(int employeeId)
        {
            return await _context.Assignments
                .Where(a => a.EmployeeId == employeeId && a.EndDate == null) // Only active assignments
                .Include(a => a.Taxi)
                .ToListAsync();
        }

        public async Task<bool> IsEmployeeAssignedToTaxiAsync(int employeeId, int taxiId)
        {
            return await _context.Assignments
                .AnyAsync(a => a.EmployeeId == employeeId && 
                              a.TaxiId == taxiId && 
                              a.EndDate == null); // Only active assignments
        }
    }
}
