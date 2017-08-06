using System;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;
using AllComponentsEWStrategy = lib.Strategies.EdgeWeighting.AllComponentsEWStrategy;
using BiggestComponentEWStrategy = lib.Strategies.EdgeWeighting.BiggestComponentEWStrategy;

namespace lib.Strategies.StrategiesCatalog
{
    public class StrategyFactory
    {
        public StrategyFactory(string name, Func<State, IServices, IStrategy> strategyProvider)
        {
            Name = name;
            StrategyProvider = strategyProvider;
        }

        public string Name { get; }
        public Func<State, IServices, IStrategy> StrategyProvider { get; }

        public static StrategyFactory Create<TStrategy>(Func<State, IServices, TStrategy> strategyProvider)
            where TStrategy : IStrategy
        {
            return new StrategyFactory(typeof(TStrategy).GetType().Name, (s, ss) => strategyProvider(s, ss));
        }

        public static StrategyFactory ForBiggestComponentEW<TEdgeWeighter>(Func<State, IServices, TEdgeWeighter> edgeWeighterProvider)
            where TEdgeWeighter : IEdgeWeighter
        {
            return new StrategyFactory(
                $"{nameof(BiggestComponentEWStrategy)}_{typeof(TEdgeWeighter).GetType().Name}",
                BiggestComponentEWStrategy((s, ss) => edgeWeighterProvider(s, ss)));
        }

        public static StrategyFactory ForAllComponentsEW<TEdgeWeighter>(Func<State, IServices, TEdgeWeighter> edgeWeighterProvider)
            where TEdgeWeighter : IEdgeWeighter
        {
            return new StrategyFactory(
                $"{nameof(AllComponentsEWStrategy)}_{typeof(TEdgeWeighter).GetType().Name}",
                BiggestComponentEWStrategy((s, ss) => edgeWeighterProvider(s, ss)));
        }
    }
}