using System;
using System.Linq;
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

        protected CompositeStrategicAi(Func<State, IServices, ISetupStrategy> setupStrategyProvider, params Func<State, IServices, IStrategy>[] strategyProviders)
        {
            SetupStrategyProvider = setupStrategyProvider;
            StrategyProviders = strategyProviders;
        }

        private Func<State, IServices, ISetupStrategy> SetupStrategyProvider { get; }
        private Func<State, IServices, IStrategy>[] StrategyProviders { get; }

        public string Name => GetType().Name;
        public abstract string Version { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            foreach (var strategyProvider in StrategyProviders)
                strategyProvider(state, services);
            var setupDecision = SetupStrategyProvider?.Invoke(state, services)?.Setup();
            if (!state.settings.futures)
                return AiSetupDecision.Empty();
            return setupDecision ?? AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            foreach (var strategyProvider in StrategyProviders)
            {
                var strategy = strategyProvider(state, services);
                var turns = strategy.NextTurns();
                if (!turns.Any())
                    continue;
                return turns.MaxBy(x => x.Estimation).Move;
            }
            return AiMoveDecision.Pass(state.punter);
        }
    }
}