using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StatisticsService.Models;

namespace StatisticsService.Controllers
{
    [Route("api/[controller].json")]
    [EnableCors("SiteCorsPolicy")]
    public class ReplaysController : Controller
    {
        private readonly IReplaysStatisticsProvider replaysStatisticsProvider;

        public ReplaysController()
        {
            replaysStatisticsProvider = new ReplaysStatisticsProvider();
        }

        [HttpGet]
        public object Get()
        {
            //return new ReplayStatisticsRepo().GetMapSizes();

            return replaysStatisticsProvider.Get();
        }
    }

    [Route("api/[controller].json")]
    public class TestController : Controller
    {
        private readonly IReplayStatisticsRepo replayStatisticsRepo;

        public TestController()
        {
            replayStatisticsRepo = new ReplayStatisticsRepo();
        }

        [HttpGet]
        public object Get()
        {
            return MapMapper.Hashes;
        }
    }
}