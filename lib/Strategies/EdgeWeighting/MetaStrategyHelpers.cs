using System;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public interface IMetaStrategy
    {
    }

    public static class MetaStrategyHelpers
    {
        public static Func<State, IServices, IStrategy> BiggestComponentEWStrategy(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider, double optionsPenaltyMultiplier = 1.0)
        {
            return (state, services) => new BiggestComponentEWStrategy(edgeWeighterProvider(state, services), state, services, optionsPenaltyMultiplier);
        }

        public static Func<State, IServices, IStrategy> AllComponentsEWStrategy(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider, double optionsPenaltyMultiplier = 1.0)
        {
            return (state, services) => new AllComponentsEWStrategy(edgeWeighterProvider(state, services), state, services, optionsPenaltyMultiplier);
        }
    }
}