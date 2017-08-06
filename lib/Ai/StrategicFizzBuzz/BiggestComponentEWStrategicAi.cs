using System;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class BiggestComponentEWStrategicAi : StrategicAi
    {
        protected BiggestComponentEWStrategicAi(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider)
            : base(BiggestComponentEWStrategy(edgeWeighterProvider))
        {
        }
    }
}