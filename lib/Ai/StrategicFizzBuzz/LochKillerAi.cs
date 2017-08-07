using System;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.viz;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    [ShouldNotRunOnline] // T-16:30
    public class LochKillerAi : CompositeStrategicAi
    {
        public LochKillerAi()
            : base(
                AllComponentsEWStrategy((state, services) => new LochKillerEdgeWeighter(state, services)),
                (state, services) => new GreedyStrategy(true, state, services, Math.Max))
        {
        }

        public override string Version => "0.1.1";
    }
}