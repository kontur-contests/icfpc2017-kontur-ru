using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using lib.Structures;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class RandomEdgeWeightingAi : EdgeWeightingStrategicAi
    {
        public override string Name => nameof(RandomEdgeWeightingAi);
        public override string Version => "1.0";

        protected override IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings)
        {
            return new RandomEdgeWeighter();
        }
    }
}