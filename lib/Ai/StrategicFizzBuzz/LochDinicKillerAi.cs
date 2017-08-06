using lib.Ai.StrategicFizzBuzz;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai
{
    public class LochDinicKillerAi : CompositeStrategicAi
    {
        public LochDinicKillerAi()
            : base(
                (state, services) => new LochDinicKillerStrategy(state, services),
                (state, services) => new AllComponentsEWStrategy(new MaxVertextWeighter(100, state, services), state, services))
        {
        }

        public override string Version => "0.4";
    }
}