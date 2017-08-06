using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using lib.viz;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline] // T-16:30
    public class AllComponentsMaxReachableVertexWeightAi : AllComponentsEWStrategicAi
    {
        public AllComponentsMaxReachableVertexWeightAi() : this(100)
        {
        }

        public AllComponentsMaxReachableVertexWeightAi(double mineWeight)
            : base((state, services) => new MaxVertextWeighter(mineWeight, state, services))
        {
        }

        public override string Version => "1.1";
    }
}