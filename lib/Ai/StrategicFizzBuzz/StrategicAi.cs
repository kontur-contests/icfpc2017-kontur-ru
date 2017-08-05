using System;
using System.Linq;
using lib.GraphImpl;
using lib.Strategies;
using MoreLinq;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class StrategicAi : IAi
    {
        private int PunterId { get; set; }
        protected abstract IStrategy Strategy { get; }
        public abstract string Name { get; }

        public virtual Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            PunterId = punterId;
            return new Future[0];
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var turns = Strategy.Turn(new Graph(map));
            if (!turns.Any())
                return new PassMove(PunterId);
            var bestTurn = turns.MaxBy(x => x.Estimation);
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