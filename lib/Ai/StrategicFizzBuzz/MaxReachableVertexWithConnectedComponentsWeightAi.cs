using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using lib.Structures;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWithConnectedComponentsWeightAi : EdgeWeightingStrategicAi
    {
        public MaxReachableVertexWithConnectedComponentsWeightAi() : this(100)
        {
        }

        public MaxReachableVertexWithConnectedComponentsWeightAi(double mineWeight)
        {
            MineWeight = mineWeight;
        }

        public override string Name => nameof(MaxReachableVertexWithConnectedComponentsWeightAi);
        public override string Version => "1.0";

        private double MineWeight { get; }

        protected override IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings)
        {
            return new MaxVertextWeighterWithConnectedComponents(map, MineWeight);
        }
    }
}