using System;
using System.Linq;
using lib.StateImpl;

namespace lib.Ai
{
    public class DummyAi : IAi
    {
        private readonly double moveProbability;

        public DummyAi(double moveProbability)
        {
            this.moveProbability = moveProbability;
        }

        public string Name { get; set; } = "Dummy";
        public string Version => "0.1";

        public AiSetupDecision Setup(State state, IServices services)
        {
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var random = new Random();

            if (random.NextDouble() < moveProbability)
            {
                var river = state.map.Rivers
                    .Where(e => e.Owner == -1)
                    .Shuffle(random)
                    .FirstOrDefault();

                if (river != null)
                    return AiMoveDecision.Claim(state.punter, river.Source, river.Target, moveProbability.ToString());
            }

            return AiMoveDecision.Pass(state.punter);
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