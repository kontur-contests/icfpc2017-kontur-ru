using System;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class BiggestComponentEWStrategicAi : StrategicAi
    {
        protected BiggestComponentEWStrategicAi(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider)
            : base((state, services) => new BiggestComponentEWStrategy(state.punter, edgeWeighterProvider(state, services), services.Get<MineDistCalculator>(state)))
        {
        }
    }
}