using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class RandomEWAi : BiggestComponentEWStrategicAi
    {
        public RandomEWAi()
            : base((state, services) => new RandomEdgeWeighter())
        {
        }

        public override string Version => "1.0";
    }
}