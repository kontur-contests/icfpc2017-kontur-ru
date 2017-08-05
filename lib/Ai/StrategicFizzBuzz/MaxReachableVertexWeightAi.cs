using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWeightAi : EdgeWeightingStrategicAi
    {
        public MaxReachableVertexWeightAi() : this(100)
        {
        }

        public MaxReachableVertexWeightAi(double mineWeight)
            : base(s => new MaxVertextWeighter(s.Map, mineWeight))
        {
        }

        public override string Name => nameof(MaxReachableVertexWeightAi);
        public override string Version => "1.0";
    }
}