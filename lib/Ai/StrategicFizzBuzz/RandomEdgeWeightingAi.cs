using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class RandomEdgeWeightingAi : EdgeWeightingStrategicAi
    {
        public override string Name => nameof(RandomEdgeWeightingAi);

        protected override IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings)
        {
            return new RandomEdgeWeighter();
        }
    }
}