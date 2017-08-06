using System.Collections.Generic;
using System.Linq;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.StrategiesCatalog.StrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public static class DebutStrategies
    {
        public static readonly Dictionary<string, StrategyFactory> Factories = new[]
        {
            Create((s, ss) => new NopStrategy()),
            Create((s, ss) => new CrazyStrategy(s, ss)),
            Create((s, ss) => new FutureIsNowStrategy(s, ss)),
            Create((s, ss) => new LochDinicKillerStrategy(s, ss)),
            Create((s, ss) => new MeetInTheMiddleStrategy(s, ss)),
            Create((s, ss) => new ExtendComponentStrategy(s, ss)),
            Create((s, ss) => new BuildNewComponentStrategy(s, ss)),
            ForAllComponentsEW((s, ss) => new LochKillerEdgeWeighter(s, ss)),
        }.ToDictionary(f => f.Name);
    }
}