using System.Collections.Generic;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.Strategies.EdgeWeighting;
using lib.Strategies.lib.Strategies;
using static lib.Strategies.StrategiesCatalog.StrategyFactory;

namespace lib.Strategies.StrategiesCatalog
{
    public static class DebutStrategies
    {
        public static readonly Dictionary<string, StrategyFactory> Factories = new[]
        {
            Create((s, ss) => new NopStrategy()),
            //Create((s, ss) => new CrazyStrategy(s, ss)),
            //Create((s, ss) => new FutureIsNowStrategy(false, s, ss)),
            Create((s, ss) => new FutureIsNowStrategy(true, s, ss), "options-"),
            Create((s, ss) => new LochDinicKillerStrategy(s, ss)),
            Create((s, ss) => new AntiLochDinicStrategy(true, s, ss), "options-"), 
            //Create((s, ss) => new MeetInTheMiddleStrategy(s, ss)),
            //Create((s, ss) => new ExtendComponentStrategy(false, s, ss)),
            //Create((s, ss) => new BuildNewComponentStrategy(false, s, ss)),
            Create((s, ss) => new ExtendComponentStrategy(true, s, ss), "options-"),
            Create((s, ss) => new BuildNewComponentStrategy(true, s, ss), "options-"),
            ForAllComponentsEW((s, ss) => new LochKillerEdgeWeighter(s, ss)),
        }.ToDictionary(f => f.Name);
    }
}