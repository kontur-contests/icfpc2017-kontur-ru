using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using lib.viz;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline]
    public class RandomEWAi : BiggestComponentEWStrategicAi
    {
        public RandomEWAi()
            : base((state, services) => new RandomEdgeWeighter())
        {
        }

        public override string Version => "1.0";
    }
}