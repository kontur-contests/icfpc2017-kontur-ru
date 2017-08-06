using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochMaxVertexWeighterKillerAi : CompositeStrategicAi
    {
        public LochMaxVertexWeighterKillerAi()
            : base(
                (state, services) => new AllComponentsEWStrategy(new LochKillerEdgeWeighter(state, services), state, services),
                (state, services) => new BiggestComponentEWStrategy(new MaxVertextWeighter(100, state, services), state, services))
        {
        }

        public override string Version => "0.2";
    }
}