using Enums;
using metiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using taxi.Interfaces.Repositories;
using taxi.DTO.Expenses;
using taxi.Data;
using Microsoft.EntityFrameworkCore;

namespace taxi.Controllers
{
    [ApiController]
    [Route("api/expenses")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ApplicationContext _context;

        public ExpenseController(IExpenseRepository expenseRepository, ITaxiRepository taxiRepository, IAssignmentRepository assignmentRepository, ApplicationContext context)
        {
            _expenseRepository = expenseRepository;
            _assignmentRepository = assignmentRepository;
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseCreateDTO dto)
        {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                // Find taxi by plate number
                var taxi = await _context.Taxis
                    .FirstOrDefaultAsync(t => t.PlateNumber == dto.PlateNumber);

                if (taxi == null)
                {
                    return BadRequest(new { message = "Taxi not found" });
                }

                // Check permissions
                if (userRole == UserRoles.BOSS)
                {
                    // Boss can only add expenses to their own taxis
                    if (taxi.BossId != userId)
                    {
                        return BadRequest(new { message = "You don't have permission to add expenses for this taxi" });
                    }
                }
                else if (userRole == UserRoles.EMPLOYEE)
                {
                    // Employee can only add expenses to taxis they are assigned to
                    var isAssigned = await _assignmentRepository.IsEmployeeAssignedToTaxiAsync(userId, taxi.Id);
                    if (!isAssigned)
                    {
                        return BadRequest(new { message = "You are not assigned to this taxi" });
                    }
                }
                else
                {
                    return Forbid("Invalid role");
                }

                var expense = new Expense
                {
                    PlateNumber = dto.PlateNumber,
                    TaxiId = taxi.Id, // Set the TaxiId foreign key
                    Amount = dto.Amount,
                    ExpenseDate = dto.ExpenseDate,
                    Type = dto.Type,
                    Description = dto.Description
                };

                await _expenseRepository.AddAsync(expense);

            return Ok(new ExpenseCreateDTO
            {
                
                PlateNumber = expense.PlateNumber,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Type = expense.Type,
                Description = expense.Description
            });
        }

        // GET: api/expenses
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyExpenses()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                
                List<Expense> expenses;
                
                if (userRole == UserRoles.BOSS)
                {
                    expenses = await _expenseRepository.GetByBossAsync(userId);
                }
                else if (userRole == UserRoles.EMPLOYEE)
                {
                    // Get expenses for taxis assigned to the employee
                    var assignments = await _assignmentRepository.GetByEmployeeAsync(userId);
                    var assignedTaxiIds = assignments.Select(a => a.TaxiId).ToList();
                    
                    if (!assignedTaxiIds.Any())
                    {
                        expenses = new List<Expense>();
                    }
                    else
                    {
                        expenses = await _context.Expenses
                            .Where(e => assignedTaxiIds.Contains(e.TaxiId))
                            .Include(e => e.Taxi)
                            .OrderByDescending(e => e.ExpenseDate)
                            .ThenBy(e => e.PlateNumber)
                            .ToListAsync();
                    }
                }
                else
                {
                    return Forbid("Invalid role");
                }
                
                // Map to DTOs to avoid circular reference issues
                var expenseDTOs = expenses.Select(e => new ExpenseResponseDTO
                {
                    
                    PlateNumber = e.PlateNumber,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    Type = e.Type,
                    Description = e.Description
                }).ToList();

                return Ok(expenseDTOs);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                }
                return BadRequest(new { message = errorMessage });
            }
        }
    }
}