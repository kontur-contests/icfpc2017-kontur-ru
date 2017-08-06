using System;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochKillerAi : CompositeStrategicAi
    {
        public LochKillerAi()
            : base(
                (state, services) => new AllComponentsEWStrategy(new LochKillerEdgeWeighter(state, services), state, services),
                (state, services) => new GreedyStrategy(state, services, Math.Max))
        {
        }

        public override string Version => "0.1.1";
    }
}