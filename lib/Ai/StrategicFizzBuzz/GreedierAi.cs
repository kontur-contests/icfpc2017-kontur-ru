using lib.Strategies;

namespace lib.Ai.StrategicFizzBuzz
{
    public class GreedierAi : StrategicAi
    {
        public GreedierAi() : base((state, services) => new GreedyStrategy(state, services, (fromScore, toScore) => fromScore + toScore))
        {
        }

        public override string Version => "1.0";
    }
}