using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace StatisticsService.Controllers
{
    [Route("api/[controller].json")]
    [EnableCors("SiteCorsPolicy")]
    public class HealthController : Controller
    {
        public object Get()
        {
            return new object();
        }
    }
}