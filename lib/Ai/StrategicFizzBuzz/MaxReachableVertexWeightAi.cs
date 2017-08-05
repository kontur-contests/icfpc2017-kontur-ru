using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWeightAi : EdgeWeightingStrategicAi
    {
        public override string Name => nameof(MaxReachableVertexWeightAi);

        public int punterId { get; private set; }

        protected override IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            return new MaxVertextWeighterWithConnectedComponents(map);
        }
    }
}