using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class RandomEdgeWeightingAi : EdgeWeightingStrategicAi
    {
        public RandomEdgeWeightingAi()
            : base(s => new RandomEdgeWeighter())
        {
        }

        public override string Name => nameof(RandomEdgeWeightingAi);
        public override string Version => "1.0";
    }
}