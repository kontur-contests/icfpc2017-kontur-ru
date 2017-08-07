using JetBrains.Annotations;
using lib.Strategies;
using lib.viz;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline]
    public class GreedierAi : StrategicAi
    {
        public GreedierAi() : base((state, services) => new GreedyStrategy(true, state, services, (fromScore, toScore) => fromScore + toScore))
        {
        }

        public override string Version => "1.0";
    }
}