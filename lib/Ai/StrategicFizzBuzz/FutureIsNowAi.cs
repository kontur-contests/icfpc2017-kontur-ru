using lib.GraphImpl;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public class FutureIsNowAi : CompositeStrategicAi
    {
        public FutureIsNowAi() : this(0.2, 100)
        {
        }

        public FutureIsNowAi(double pathMultiplier, double mineWeight)
            : base(
                (state, services) => new FutureIsNowStrategy(pathMultiplier, state, services.Get<Graph>(), services.Get<MineDistCalculator>()),
                (state, services) => new BiggestComponentEWStrategy(new MaxVertextWeighter(mineWeight, state, services), state, services))
        {
        }

        public override string Version => "1";
    }
}