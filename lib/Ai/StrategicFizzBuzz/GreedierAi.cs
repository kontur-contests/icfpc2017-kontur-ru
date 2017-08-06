using lib.GraphImpl;
using lib.Strategies;

namespace lib.Ai.StrategicFizzBuzz
{
    public class GreedierAi : StrategicAi
    {
        public GreedierAi() : base((state, services) => new GreedyStrategy(state.punter, services.Get<MineDistCalculator>()))
        {
        }

        public override string Version => "1.0";
    }
}