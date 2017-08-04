using System;
using System.Linq;

namespace lib.Ai
{
    public class DummyAi : IAi
    {
        private int myPunterId;
        private readonly double moveProbability;

        public DummyAi(double moveProbability)
        {
            this.moveProbability = moveProbability;
        }

        public string Name { get; } = "Dummy";

        public void StartRound(int punterId, int puntersCount, Map map)
        {
            myPunterId = punterId;
        }

        public IMove GetNextMove(IMove[] prevMoves, Map map)
        {
            var random = new Random();

            if (random.NextDouble() < moveProbability)
            {
                var river = map.Rivers
                    .Where(e => e.Owner == -1)
                    .Shuffle(random)
                    .FirstOrDefault();

                if (river != null)
                    return new Move
                    {
                        PunterId = myPunterId,
                        Source = river.Source,
                        Target = river.Target
                    };
            }

            return new Pass(myPunterId);
        }

        public string SerializeGameState()
        {
            return string.Empty;
        }

        public void DeserializeGameState(string gameState)
        {
        }
    }
}