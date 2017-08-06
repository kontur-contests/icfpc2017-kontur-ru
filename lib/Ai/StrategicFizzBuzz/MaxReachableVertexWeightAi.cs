using JetBrains.Annotations;
using lib.GraphImpl;
using lib.Strategies.EdgeWeighting;
using lib.viz;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline] // T-16:30
    public class MaxReachableVertexWeightAi : BiggestComponentEWStrategicAi
    {
        public MaxReachableVertexWeightAi() : this(100)
        {
        }

        public MaxReachableVertexWeightAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighter(mineWeight, state, services))
        {
        }

        public override string Version => "1.1";
    }
}