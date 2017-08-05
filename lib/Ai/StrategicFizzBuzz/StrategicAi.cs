using System;
using System.Linq;
using lib.GraphImpl;
using lib.Strategies;
using lib.Structures;
using MoreLinq;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class StrategicAi : IAi
    {
        protected StrategicAi(Func<SuperSettings, IStrategy> strategyProvider)
        {
            StrategyProvider = strategyProvider;
        }

        private int PunterId { get; set; }
        private IStrategy Strategy { get; set; }
        public abstract string Name { get; }
        public abstract string Version { get; }
        private Func<SuperSettings, IStrategy> StrategyProvider { get; }

        public virtual Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            Strategy = StrategyProvider(new SuperSettings(punterId, puntersCount, map, settings));
            PunterId = punterId;
            return new Future[0];
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var turns = Strategy.Turn(new Graph(map));
            if (!turns.Any())
                return Move.Pass(PunterId);
            var bestTurn = turns.MaxBy(x => x.Estimation);
            return Move.Claim(PunterId, bestTurn.River.Source, bestTurn.River.Target);
        }

        public string SerializeGameState()
        {
            return "";
        }

        public void DeserializeGameState(string gameState)
        {
        }
    }
}