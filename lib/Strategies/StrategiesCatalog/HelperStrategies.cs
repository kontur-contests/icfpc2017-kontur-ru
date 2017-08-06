using System.Collections.Generic;
using System.Linq;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.StrategiesCatalog.StrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public class HelperStrategies
    {
        public static readonly Dictionary<string, StrategyFactory> Factories = new[]
        {
            Create((s, ss) => new NopStrategy()),
            Create((s, ss) => new ExtendComponentStrategy(s, ss)),
            Create((s, ss) => new BuildNewComponentStrategy(s, ss)),
            ForAllComponentsEW((s, ss) => new MineGuardianEdgeWeighter(s, ss)),
        }.ToDictionary(f => f.Name);
    }
}