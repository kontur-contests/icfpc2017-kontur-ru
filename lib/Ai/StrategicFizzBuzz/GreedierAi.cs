using lib.Strategies;
using lib.Structures;

namespace lib.Ai.StrategicFizzBuzz
{
    public class GreedierAi : StrategicAi
    {
        private IStrategy strategy;

        public override string Name => nameof(GreedierAi);
        public override string Version => "1.0";
        protected override IStrategy Strategy => strategy;

        public override Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            strategy = new GreedyStrategy(map, punterId);
            return base.StartRound(punterId, puntersCount, map, settings);
        }
    }
}