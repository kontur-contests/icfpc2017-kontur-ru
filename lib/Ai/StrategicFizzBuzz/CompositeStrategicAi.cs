using System;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using MoreLinq;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class CompositeStrategicAi : IAi
    {
        protected CompositeStrategicAi(params Func<State, IServices, IStrategy>[] strategyProviders)
        {
            StrategyProviders = strategyProviders;
        }

        private Func<State, IServices, IStrategy>[] StrategyProviders { get; }

        public string Name => GetType().Name;
        public abstract string Version { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<Graph>();
            StrategyProviders.Select(sp => sp(state, services)).Consume();
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            foreach (var strategyProvider in StrategyProviders)
            {
                var strategy = strategyProvider(state, services);
                var turns = strategy.NextTurns();
                if (!turns.Any())
                    continue;
                var bestTurn = turns.MaxBy(x => x.Estimation);
                return AiMoveDecision.Claim(state.punter, bestTurn.River.Source, bestTurn.River.Target);
            }
            return AiMoveDecision.Pass(state.punter);
        }
    }
}