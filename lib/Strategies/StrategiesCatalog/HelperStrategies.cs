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
            //Create((s, ss) => new ExtendComponentStrategy(false, s, ss)),
            //Create((s, ss) => new BuildNewComponentStrategy(false, s, ss)),
            Create((s, ss) => new ExtendComponentStrategy(true, s, ss), "options-"),
            Create((s, ss) => new BuildNewComponentStrategy(true, s, ss), "options-"),
            ForAllComponentsEW((s, ss) => new MineGuardianEdgeWeighter(s, ss)),
        }.ToDictionary(f => f.Name);
    }
}