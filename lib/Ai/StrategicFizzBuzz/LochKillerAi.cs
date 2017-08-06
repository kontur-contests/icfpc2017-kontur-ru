using System;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochKillerAi : CompositeStrategicAi
    {
        public LochKillerAi()
            : base(
                AllComponentsEWStrategy((state, services) => new LochKillerEdgeWeighter(state, services)),
                (state, services) => new GreedyStrategy(state, services, Math.Max))
        {
        }

        public override string Version => "0.1.1";
    }
}