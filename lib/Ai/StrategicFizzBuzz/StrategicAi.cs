using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using MoreLinq;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class StrategicAi : IAi
    {
        protected abstract IStrategy CreateStrategy(State state, IServices services);

        public string Name => GetType().Name;
        public string Version => "0.1";

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            CreateStrategy(state, services);
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var strategy = CreateStrategy(state, services);
            var graph = services.Get<GraphService>(state).Graph;
            var turns = strategy.Turn(graph);
            if (!turns.Any())
                return AiMoveDecision.Pass(state.punter);
            var bestTurn = turns.MaxBy(x => x.Estimation);
            return AiMoveDecision.Claim(state.punter, bestTurn.River.Source, bestTurn.River.Target);
        }       
    }
}