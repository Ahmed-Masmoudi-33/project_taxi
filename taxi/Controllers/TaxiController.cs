using Enums;
using metiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using taxi.Data;
using taxi.DTO.Taxi;
using taxi.Interfaces.Repositories;


namespace taxi.Controllers
{
    [ApiController]
    [Route("api/taxis")]
    [Authorize] // Require authentication for all methods
    public class TaxiController : ControllerBase
    {
        private readonly ITaxiRepository _taxiRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssignmentRepository _assignmentRepository;


        public TaxiController(ITaxiRepository taxiRepository, IUserRepository userRepository, IAssignmentRepository assignmentRepository)
        {
            _taxiRepository = taxiRepository;
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
        }

        // POST: api/taxis
        [HttpPost]
        [Authorize(Roles = UserRoles.BOSS)]
        public async Task<IActionResult> CreateTaxi([FromBody] TaxiCreateDTO dto)
        {
            var bossId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


            var taxi = new Taxi
            {
                PlateNumber = dto.PlateNumber,
                Governorate = dto.Governorate,
                BossId = bossId
            };

            await _taxiRepository.AddAsync(taxi);

            // If EmployeeCIN is provided, assign the employee to this taxi
            if (!string.IsNullOrWhiteSpace(dto.EmployeeCIN))
            {
                var employee = await _userRepository.GetByCINAsync(dto.EmployeeCIN);
                if (employee == null)
                {
                    return BadRequest(new { message = $"Employee with CIN '{dto.EmployeeCIN}' not found." });
                }

                if (employee.Role != UserRoles.EMPLOYEE)
                {
                    return BadRequest(new { message = $"User with CIN '{dto.EmployeeCIN}' is not an employee." });
                }

                // Check if employee already has an active assignment for this taxi
                var existingAssignments = await _assignmentRepository.GetByEmployeeAsync(employee.Id);
                if (existingAssignments.Any(a => a.TaxiId == taxi.Id && a.EndDate == null))
                {
                    return BadRequest(new { message = "This employee is already assigned to this taxi." });
                }

                // Create assignment
                var assignment = new Assignment
                {
                    TaxiId = taxi.Id,
                    EmployeeId = employee.Id,
                    StartDate = DateTime.Now
                };

                await _assignmentRepository.AddAsync(assignment);
            }

            return Ok(new TaxiResponseDTO
            {
                PlateNumber = taxi.PlateNumber,
                Governorate = taxi.Governorate
                

            });
        }

        // PUT: api/taxis/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.BOSS)]
        public async Task<IActionResult> UpdateTaxi(int id, [FromBody] TaxiUpdateDTO dto)
        {
            var bossId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var taxi = await _taxiRepository.GetByIdAsync(id);
            if (taxi == null)
                return NotFound();

            // Ownership check
            if (taxi.BossId != bossId)
                return Forbid();

            // Update taxi properties
            taxi.PlateNumber = dto.PlateNumber;
            taxi.Governorate = dto.Governorate;

            // Handle employee assignment if EmployeeCIN is provided
            if (!string.IsNullOrWhiteSpace(dto.EmployeeCIN))
            {
                // Check if employee with this CIN exists in users table
                var employee = await _userRepository.GetByCINAsync(dto.EmployeeCIN);
                if (employee == null)
                {
                    return BadRequest(new { message = $"Employee with CIN '{dto.EmployeeCIN}' not found." });
                }

                // End any current active assignment for this taxi
                var currentAssignment = taxi.Assignments?.FirstOrDefault(a => a.EndDate == null);
                if (currentAssignment != null)
                {
                    await _assignmentRepository.EndAssignmentAsync(currentAssignment.Id, DateTime.Now);
                }

                // Create a new assignment matching the employee ID to the taxi ID
                var newAssignment = new Assignment
                {
                    TaxiId = taxi.Id,
                    EmployeeId = employee.Id,
                    StartDate = DateTime.Now
                };

                await _assignmentRepository.AddAsync(newAssignment);
            }

            // Update taxi
            await _taxiRepository.UpdateAsync(taxi);

            // Reload taxi with assignments to get the employee name
            var updatedTaxi = await _taxiRepository.GetByIdAsync(id);
            if (updatedTaxi == null)
            {
                return NotFound();
            }

            // Get the active assignment (if any) to display employee name
            var activeAssignment = updatedTaxi.Assignments?.FirstOrDefault(a => a.EndDate == null);
            
            // Get employee name: FirstName + LastName
            string? employeeName = null;
            if (activeAssignment != null && activeAssignment.Employee != null)
            {
                employeeName = $"{activeAssignment.Employee.FirstName} {activeAssignment.Employee.LastName}";
            }

            return Ok(new TaxiResponseDTO
            {
                Id = updatedTaxi.Id,
                PlateNumber = updatedTaxi.PlateNumber,
                Governorate = updatedTaxi.Governorate,
                AssignedEmployeeName = employeeName,
                AssignedEmployeeCIN = activeAssignment?.Employee?.CIN
            });
        }

        // GET: api/taxis
        [HttpGet]
        [Authorize(Roles = UserRoles.BOSS)]
        public async Task<IActionResult> GetMyTaxis()
            {
                var bossId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var taxis = await _taxiRepository.GetAllByBossAsync(bossId);

                // Map to DTOs with assigned employee name
                var taxiDTOs = taxis.Select(t =>
                {
                    // Find the active assignment (EndDate is null)
                    var activeAssignment = t.Assignments?.FirstOrDefault(a => a.EndDate == null);
                    var employeeName = activeAssignment?.Employee != null
                        ? $"{activeAssignment.Employee.FirstName} {activeAssignment.Employee.LastName}"
                        : null;
                    var employeeCIN = activeAssignment?.Employee?.CIN;

                    return new TaxiResponseDTO
                    {
                        Id = t.Id,
                        PlateNumber = t.PlateNumber,
                        Governorate = t.Governorate,
                        AssignedEmployeeName = employeeName,
                        AssignedEmployeeCIN = employeeCIN
                    };
                }).ToList();

                return Ok(taxiDTOs);
            }
        
        // GET: api/taxis/my-assigned (for employees)
        [HttpGet("my-assigned")]
        [Authorize(Roles = UserRoles.EMPLOYEE)]
        public async Task<IActionResult> GetMyAssignedTaxis()
        {
            var employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var assignments = await _assignmentRepository.GetByEmployeeAsync(employeeId);
            
            // Map assignments to taxi DTOs
            var taxiDTOs = assignments.Select(a => new TaxiResponseDTO
            {
                Id = a.Taxi.Id,
                PlateNumber = a.Taxi.PlateNumber,
                Governorate = a.Taxi.Governorate,
                AssignedEmployeeName = null, // Not needed for employee view
                AssignedEmployeeCIN = null // Not needed for employee view
            }).ToList();

            return Ok(taxiDTOs);
        }
    }
}
