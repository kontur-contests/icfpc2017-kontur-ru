using JetBrains.Annotations;
using lib.GraphImpl;
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
            : base((state, services) => new MaxVertextWeighter(mineWeight, services.Get<MineDistCalculator>(state)))
        {
        }

        public override string Version => "1.0";
    }
}