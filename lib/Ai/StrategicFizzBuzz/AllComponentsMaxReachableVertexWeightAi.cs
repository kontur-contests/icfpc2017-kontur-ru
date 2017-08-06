using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class AllComponentsMaxReachableVertexWeightAi : AllComponentsEWStrategicAi
    {
        public AllComponentsMaxReachableVertexWeightAi() : this(100)
        {
        }

        public AllComponentsMaxReachableVertexWeightAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighter(mineWeight, state, services))
        {
        }

        public override string Version => "1.0";
    }
}