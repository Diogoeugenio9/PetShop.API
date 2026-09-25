using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Dashboard;
using PetShop.API.Services.Dashboard;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("Stats")]
        public async Task<ActionResult<DashboardStatsDto>> Stats()
        {
            var stats = await _dashboardService.ObterStats();

            return Ok(stats);
        }
    }
}