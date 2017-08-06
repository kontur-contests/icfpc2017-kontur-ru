using JetBrains.Annotations;
using lib.GraphImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class AllComponentsMaxReachableVertexWeightWCCAi : AllComponentsEWStrategicAi
    {
        public AllComponentsMaxReachableVertexWeightWCCAi() : this((double) 100)
        {
        }

        public AllComponentsMaxReachableVertexWeightWCCAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighterWithConnectedComponents(mineWeight, services.Get<MineDistCalculator>(state)))
        {
        }

        public override string Version => "1.0";
    }
}