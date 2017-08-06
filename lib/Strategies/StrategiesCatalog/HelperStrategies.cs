using lib.Strategies.EdgeWeighting;
using static lib.Strategies.StrategiesCatalog.StrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public class HelperStrategies
    {
        public static StrategyFactory[] Factories =
        {
            Create((s, ss) => new NopStrategy()),
            ForAllComponentsEW((s, ss) => new MineGuardianEdgeWeighter(s, ss)),
        };
    }
}