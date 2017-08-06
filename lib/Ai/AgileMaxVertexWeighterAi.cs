using JetBrains.Annotations;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai
{
    [UsedImplicitly]
    public class AgileMaxVertexWeighterAi : AllComponentsEWStrategicAi
    {
        public AgileMaxVertexWeighterAi() : this(100)
        {
        }

        public AgileMaxVertexWeighterAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighter(mineWeight, services.Get<MineDistCalculator>(state)))
        {
        }

        public override string Version => "1.1";
    }
}