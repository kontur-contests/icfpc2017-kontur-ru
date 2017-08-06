using System;
using System.Data;
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

        private readonly object locker = new object();
        private ReplaysStatistics data;
        private DateTime lastUpdateTime = DateTime.UtcNow;

        [HttpGet]
        public object Get()
        {
            if (data != null && DateTime.UtcNow - lastUpdateTime <= TimeSpan.FromMinutes(4))
            {
                return data;
            }

            lock (locker)
            {
                lastUpdateTime = DateTime.UtcNow;
                data = replaysStatisticsProvider.Get();
                return data;
            }
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