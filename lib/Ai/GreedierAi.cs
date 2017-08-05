using System;
using System.Linq;
using lib.GraphImpl;
using lib.Strategies;
using MoreLinq;

namespace lib.Ai
{
    public class GreedierAi : IAi
    {
        private GreedyStrategy Strategy { get; set; }
        private int PunterId { get; set; }

        public string Name => nameof(GreedierAi);
        public void StartRound(int punterId, int puntersCount, Map map)
        {
            PunterId = punterId;
            Strategy = new GreedyStrategy(map, punterId);
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var turns = Strategy.Turn(new Graph(map));
            if (!Enumerable.Any<TurnResult>(turns))
                return new PassMove(PunterId);
            var bestTurn = MoreEnumerable.MaxBy<TurnResult, double>(turns, x => x.Estimation);
            return new ClaimMove(PunterId, bestTurn.River.Source, bestTurn.River.Target);
        }

        public string SerializeGameState()
        {
            throw new NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new NotImplementedException();
        }
    }
}