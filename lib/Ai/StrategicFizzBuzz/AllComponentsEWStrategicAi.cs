using System;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class AllComponentsEWStrategicAi : StrategicAi
    {
        protected AllComponentsEWStrategicAi(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider)
            : base((state, services) => new AllComponentsEWStrategy(state.punter, edgeWeighterProvider(state, services), services.Get<MineDistCalculator>()))
        {
        }
    }
}