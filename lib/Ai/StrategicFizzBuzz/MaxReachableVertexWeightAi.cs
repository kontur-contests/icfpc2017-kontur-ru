using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWeightAi : EdgeWeightingStrategicAi
    {
        public override string Name => nameof(MaxReachableVertexWeightAi);

        protected override IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings)
        {
            return new SubGraphEdgeWeighter(map);
        }
    }
}