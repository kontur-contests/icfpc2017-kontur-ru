using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using lib.Structures;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWeightAi : EdgeWeightingStrategicAi
    {
        public double MineWeight { get; }

        public MaxReachableVertexWeightAi() : this(100)
        {
        }
        
        public MaxReachableVertexWeightAi(double mineWeight)
        {
            MineWeight = mineWeight;
        }

        public override string Name => nameof(MaxReachableVertexWeightAi);
        public override string Version => "1.0";
        public int punterId { get; private set; }

        protected override IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            return new MaxVertextWeighterWithConnectedComponents(map, MineWeight);
        }
    }
}