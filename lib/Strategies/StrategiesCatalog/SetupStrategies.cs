using System.Collections.Generic;
using System.Linq;
using static lib.Strategies.StrategiesCatalog.SetupStrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public static class SetupStrategies
    {
        public static readonly Dictionary<string, SetupStrategyFactory> Factories = new[]
            {
                Create((s, ss) => new NopSetupStrategy()),
                Create((s, ss) => new FutureIsNowSetupStrategy(s, ss)),
                //Create((s, ss) => new MeetInTheMiddleSetupStrategy(s, ss)),
            }.ToDictionary(f => f.Name);
    }
}