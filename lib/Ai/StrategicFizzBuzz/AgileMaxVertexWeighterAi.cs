using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class AgileMaxVertexWeighterAi : AllComponentsEWStrategicAi
    {
        public AgileMaxVertexWeighterAi() : this(100)
        {
        }

        public AgileMaxVertexWeighterAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighter(mineWeight, state, services))
        {
        }

        public override string Version => "1.2";
    }
}