using JetBrains.Annotations;
using lib.GraphImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWithConnectedComponentsWeightAi : BiggestComponentEWStrategicAi
    {
        public MaxReachableVertexWithConnectedComponentsWeightAi() : this(100)
        {
        }

        public MaxReachableVertexWithConnectedComponentsWeightAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighterWithConnectedComponents(mineWeight, services.Get<MineDistCalculator>(state)))
        {
        }

        public override string Version => "1.0";
    }
}