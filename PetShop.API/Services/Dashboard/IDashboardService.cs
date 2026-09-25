using PetShop.API.Dto.Dashboard;

namespace PetShop.API.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> ObterStats();
    }
}