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
        [HttpGet]
        public object Get()
        {
            return new object();
        }
    }
}