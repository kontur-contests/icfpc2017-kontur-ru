using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochMaxVertexWeighterKillerAi : CompositeStrategicAi
    {
        public LochMaxVertexWeighterKillerAi()
            : base(
                AllComponentsEWStrategy((state, services) => new LochKillerEdgeWeighter(state, services)),
                BiggestComponentEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)))
        {
        }

        public override string Version => "0.2";
    }
}