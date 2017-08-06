using System;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.StrategiesCatalog.StrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public class MittelspielStrategies
    {
        public static StrategyFactory[] Factories =
        {
            Create((s, ss) => new GreedyStrategy(s, ss, Math.Max), "Max"),
            Create((s, ss) => new GreedyStrategy(s, ss, (x, y) => x + y), "Sum"),
            ForBiggestComponentEW((s, ss) => new MaxVertextWeighter(100, s, ss)),
            ForBiggestComponentEW((s, ss) => new RandomEdgeWeighter()),
            ForAllComponentsEW((s, ss) => new MaxVertextWeighter(100, s, ss)),
        };
    }
}