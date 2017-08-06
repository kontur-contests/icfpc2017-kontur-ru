using System;
using System.Collections.Generic;
using System.Linq;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.StrategiesCatalog.StrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public class MittelspielStrategies
    {
        public static readonly Dictionary<string, StrategyFactory> Factories = new[]
        {
            Create((s, ss) => new NopStrategy()),
            Create((s, ss) => new GreedyStrategy(s, ss, Math.Max), "Max"),
            Create((s, ss) => new GreedyStrategy(s, ss, (x, y) => x + y), "Sum"),
            ForBiggestComponentEW((s, ss) => new MaxVertextWeighter(100, s, ss)),
            ForAllComponentsEW((s, ss) => new MaxVertextWeighter(100, s, ss)),
        }.ToDictionary(f => f.Name);
    }
}