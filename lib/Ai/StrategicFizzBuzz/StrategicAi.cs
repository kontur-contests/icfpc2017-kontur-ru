using System;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using MoreLinq;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class StrategicAi : IAi
    {
        protected StrategicAi(Func<State, IServices, IStrategy> strategyProvider)
        {
            StrategyProvider = strategyProvider;
        }

        public string Name => GetType().Name;
        public abstract string Version { get; }

        private Func<State, IServices, IStrategy> StrategyProvider { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            StrategyProvider(state, services);
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var turns = StrategyProvider(state, services).NextTurns();
            if (!turns.Any())
                return AiMoveDecision.Pass(state.punter);
            var bestTurn = turns.MaxBy(x => x.Estimation);
            return AiMoveDecision.Claim(state.punter, bestTurn.River.Source, bestTurn.River.Target);
        }
    }
}