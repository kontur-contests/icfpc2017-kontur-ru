using System;
using System.Linq;
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

        private Func<State, IServices, IStrategy> StrategyProvider { get; }

        public string Name => GetType().Name;
        public abstract string Version { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            var strategy = StrategyProvider(state, services);
            var setupStrategy = strategy as ISetupStrategy;
            var decision = setupStrategy?.Setup();
            if (!state.settings.futures)
                return AiSetupDecision.Empty();
            return decision ?? AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var strategy = StrategyProvider(state, services);
            var turns = strategy.NextTurns();
            if (!turns.Any())
                return AiMoveDecision.Pass(state.punter);
            return turns.MaxBy(x => x.Estimation).Move;
        }
    }
}