using Microsoft.AspNetCore.Mvc;
using Unmatched.Dtos;
using Unmatched.Services.Contracts;
using Unmatched.Services.Statistics;

namespace Unmatched.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroesController : ControllerBase
    {
        private readonly IHeroService _heroService;
        private readonly IHeroStatisticsService _heroStatisticsService;

        public HeroesController(IHeroService heroService, IHeroStatisticsService heroStatisticsService)
        {
            _heroService = heroService;
            _heroStatisticsService = heroStatisticsService;
        }

        [HttpGet]
        public Task<IEnumerable<UiHeroDto>> Get()
        {
            return _heroService.GetAsync();
        }

        [HttpGet("statistics")]
        public Task<IEnumerable<UiHeroStatisticsDto>> GetStatistics()
        {
            return _heroStatisticsService.GetHeroesStatisticsAsync();
        }
    }
}
