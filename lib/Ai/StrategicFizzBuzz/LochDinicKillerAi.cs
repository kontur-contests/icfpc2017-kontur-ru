using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochDinicKillerAi : CompositeStrategicAi
    {
        public LochDinicKillerAi()
            : base(
                (state, services) => new LochDinicKillerStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)))
        {
        }

        public override string Version => "0.4";
    }
}