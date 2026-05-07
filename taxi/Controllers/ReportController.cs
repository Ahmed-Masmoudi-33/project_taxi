using Enums;
using metiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using taxi.DTO.Reports;
using taxi.DTO.Ride;
using taxi.Interfaces.Repositories;
using taxi.Services;

namespace taxi.Controllers
{
    
    [ApiController]
      [Route("api/reports")]
      [Authorize]
      public class ReportController : ControllerBase
      {
          private readonly ReportService _reportService;
          private readonly ITaxiRepository _taxiRepository;
          private readonly IAssignmentRepository _assignmentRepository;

          public ReportController(ReportService reportService, ITaxiRepository taxiRepository, IAssignmentRepository assignmentRepository)
          {
              _reportService = reportService;
              _taxiRepository = taxiRepository;
              _assignmentRepository = assignmentRepository;
          }

          // GET: api/reports/taxi/{taxiId}/summary?year={year}&month={month}
          [HttpGet("taxi/{taxiId}/summary")]
          public async Task<IActionResult> GenerateAsync(int taxiId, [FromQuery] int year, [FromQuery] int month)
          {
              try
              {
                  var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                  var userRole = User.FindFirstValue(ClaimTypes.Role);

                  // Validate taxi exists
                  var taxi = await _taxiRepository.GetByIdAsync(taxiId);
                  if (taxi == null)
                      return NotFound(new { message = "Taxi not found" });

                  // Check permissions
                  if (userRole == UserRoles.BOSS)
                  {
                      // Boss can only view reports for their own taxis
                      if (taxi.BossId != userId)
                          return Forbid("You don't have access to this taxi's reports");
                  }
                  else if (userRole == UserRoles.EMPLOYEE)
                  {
                      // Employee can only view reports for taxis they are assigned to
                      var isAssigned = await _assignmentRepository.IsEmployeeAssignedToTaxiAsync(userId, taxiId);
                      if (!isAssigned)
                          return Forbid("You are not assigned to this taxi");
                  }
                  else
                  {
                      return Forbid("Invalid role");
                  }

                  // Use the boss ID for the report generation (the taxi's owner)
                  var bossId = taxi.BossId;

                  // Validate month and year
                  if (month < 1 || month > 12)
                      return BadRequest(new { message = "Month must be between 1 and 12" });

                  if (year < 2000 || year > 2100)
                      return BadRequest(new { message = "Year must be between 2000 and 2100" });

                  // Generate the report
                  var report = await _reportService.GenerateAsync(taxiId, bossId, year, month);

                  return Ok(report);
              }
              catch (Exception ex)
              {
                  return StatusCode(500, new { message = "An error occurred while generating the report", error = ex.Message });
              }
          }
      }
}

