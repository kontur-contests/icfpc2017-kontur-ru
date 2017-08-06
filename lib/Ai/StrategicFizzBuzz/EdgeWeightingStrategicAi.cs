using System;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class EdgeWeightingStrategicAi : StrategicAi
    {
        protected EdgeWeightingStrategicAi(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider)
            : base((state, services) => new EdgeWeightingStrategy(state.punter, edgeWeighterProvider(state, services), services.Get<MineDistCalculator>(state)))
        {
        }
    }
}