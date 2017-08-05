using JetBrains.Annotations;
using lib.GraphImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWithConnectedComponentsWeightAi : EdgeWeightingStrategicAi
    {
        public MaxReachableVertexWithConnectedComponentsWeightAi() : this(100)
        {
        }

        public MaxReachableVertexWithConnectedComponentsWeightAi(double mineWeight)
            : base(s => new MaxVertextWeighterWithConnectedComponents(new Graph(s.Map), mineWeight))
        {
        }

        public override string Name => nameof(MaxReachableVertexWithConnectedComponentsWeightAi);
        public override string Version => "1.0";
    }
}