using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class FutureIsNowAi : CompositeStrategicAi
    {
        public FutureIsNowAi() : this(1, 100)
        {
        }

        public FutureIsNowAi(double pathMultiplier, double mineWeight)
            : base(
                (state, services) => new FutureIsNowSetupStrategy(state, services, pathMultiplier),
                (state, services) => new FutureIsNowStrategy(true, state, services),
                BiggestComponentEWStrategy((state, services) => new MaxVertextWeighter(mineWeight, state, services)),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "1";
    }
}