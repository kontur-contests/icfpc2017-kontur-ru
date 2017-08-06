using System;
using lib.StateImpl;

namespace lib.Strategies.StrategiesCatalog
{
    public class SetupStrategyFactory
    {
        public SetupStrategyFactory(string name, Func<State, IServices, ISetupStrategy> setupStrategyProvider)
        {
            Name = name;
            SetupStrategyProvider = setupStrategyProvider;
        }

        public string Name { get; }
        public Func<State, IServices, ISetupStrategy> SetupStrategyProvider { get; }

        public static SetupStrategyFactory Create<TStrategy>(Func<State, IServices, TStrategy> strategyProvider)
            where TStrategy : ISetupStrategy
        {
            return new SetupStrategyFactory(StrategyName.ForSetup<TStrategy>(), (s, ss) => strategyProvider(s, ss));
        }
    }
}