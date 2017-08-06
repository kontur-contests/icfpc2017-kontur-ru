using System;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class BiggestComponentEWStrategicAi : StrategicAi
    {
        protected BiggestComponentEWStrategicAi(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider)
            : base((state, services) => new BiggestComponentEWStrategy(edgeWeighterProvider(state, services), state, services))
        {
        }
    }
}