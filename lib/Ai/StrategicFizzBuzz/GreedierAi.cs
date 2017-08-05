using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;

namespace lib.Ai.StrategicFizzBuzz
{
    public class GreedierAi : StrategicAi
    {
        protected override IStrategy CreateStrategy(State state, IServices services)
        {
            return new GreedyStrategy(state.punter, services.Get<MineDistCalculator>(state));
        }
    }
}