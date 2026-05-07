using Enums;
using metiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using taxi.Interfaces.Repositories;
using taxi.DTO.Ride;

namespace taxi.Controllers
{
    [ApiController]
    [Route("api/rides")]
    public class RideController : ControllerBase
    {
        private readonly IRideRepository _rideRepository;
        private readonly ITaxiRepository _taxiRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public RideController(IRideRepository rideRepository, ITaxiRepository taxiRepository, IAssignmentRepository assignmentRepository)
        {
            _rideRepository = rideRepository;
            _taxiRepository = taxiRepository;
            _assignmentRepository = assignmentRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRide([FromBody] RideCreateDTO dto)
        {

                // Both BOSS and EMPLOYEE can add rides
                // The EmployeeId will be set to the current user's ID (boss or employee)
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                // Validate taxi exists
                var taxi = await _taxiRepository.GetByIdAsync(dto.TaxiId);
                if (taxi == null)
                {
                    return BadRequest(new { message = "Taxi not found" });
                }

                // Check permissions
                if (userRole == UserRoles.BOSS)
                {
                    // Boss can only add rides to their own taxis
                    if (taxi.BossId != userId)
                    {
                        return Forbid("You can only add rides to your own taxis");
                    }
                }
                else if (userRole == UserRoles.EMPLOYEE)
                {
                    // Employee can only add rides to taxis they are assigned to
                    var isAssigned = await _assignmentRepository.IsEmployeeAssignedToTaxiAsync(userId, dto.TaxiId);
                    if (!isAssigned)
                    {
                        return Forbid("You are not assigned to this taxi");
                    }
                }

                var ride = new Ride
                {
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    DistanceKm = dto.DistanceKm,
                    Amount = dto.Amount,
                    TaxiId = dto.TaxiId,
                    EmployeeId = userId // This will be the boss's ID if boss adds the ride, or employee's ID if employee adds it
                };

                await _rideRepository.AddAsync(ride);
                
                // Return response DTO to avoid circular reference
                return Ok(new RideResponseDTO
                {
                    StartDate = ride.StartDate,
                    EndDate = ride.EndDate,
                    DistanceKm = ride.DistanceKm,
                    Amount = ride.Amount,
                    TaxiPlateNumber = taxi.PlateNumber,
                    TaxiGovernorate = taxi.Governorate
                });
            
        }

        // GET: api/rides
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyRides([FromQuery] int? taxiId = null)
        {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                
                List<Ride> rides;
                
                if (userRole == UserRoles.BOSS)
                {
                    // Boss sees rides for their taxis
                    rides = await _rideRepository.GetByBossTaxisAsync(userId, taxiId);
                }
                else
                {
                    // Employee sees their own rides
                    if (taxiId.HasValue)
                    {
                        rides = await _rideRepository.GetByEmployeeAndTaxiAsync(userId, taxiId);
                    }
                    else
                    {
                        rides = await _rideRepository.GetAllByEmployeeAsync(userId);
                    }
                }

                // Map to DTOs to avoid circular reference issues
                var rideDTOs = rides.Select(r => new RideResponseDTO
                {
                    Id = r.Id,
                    TaxiId = r.TaxiId,
                    EmployeeId = r.EmployeeId,
                    EmployeeName = r.Employee != null
                        ? $"{r.Employee.FirstName} {r.Employee.LastName}".Trim()
                        : null,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    DistanceKm = r.DistanceKm,
                    Amount = r.Amount,
                    TaxiPlateNumber = r.Taxi?.PlateNumber,
                    TaxiGovernorate = r.Taxi?.Governorate
                }).ToList();

                return Ok(rideDTOs);
        }
    }
}
