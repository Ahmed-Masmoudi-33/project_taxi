using Enums;
using metiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using taxi.Interfaces.Repositories;
using taxi.DTO.Commission;

namespace taxi.Controllers
{
    [ApiController]
    [Route("api/commission")]
    [Authorize(Roles = UserRoles.BOSS)]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionRepository _commissionRepository;

        public CommissionController(ICommissionRepository commissionRepository)
        {
            _commissionRepository = commissionRepository;
        }

        // GET: api/commission
        [HttpGet]
        public async Task<IActionResult> GetMyCommission()
        {
            var bossId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var commission = await _commissionRepository.GetByBossAsync(bossId);

            return Ok(commission?.Percentage ?? 0);
        }

        // POST or PUT: api/commission
        [HttpPost]
        public async Task<IActionResult> SetCommission([FromBody] CommissionDTO dto)
        {
            if (dto.Percentage < 0 || dto.Percentage > 100)
                return BadRequest("Commission must be between 0 and 100");

            var bossId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var commission = await _commissionRepository.GetByBossAsync(bossId);

            if (commission == null)
            {
                commission = new Commission
                {
                    BossId = bossId,
                    Percentage = dto.Percentage
                };

                await _commissionRepository.AddAsync(commission);
            }
            else
            {
                commission.Percentage = dto.Percentage;
                await _commissionRepository.UpdateAsync(commission);
            }

            return Ok(commission);
        }
    }
}
