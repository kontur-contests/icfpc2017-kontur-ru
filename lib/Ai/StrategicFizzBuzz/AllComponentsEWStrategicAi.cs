using System;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class AllComponentsEWStrategicAi : StrategicAi
    {
        protected AllComponentsEWStrategicAi(Func<State, IServices, IEdgeWeighter> edgeWeighterProvider)
            : base(AllComponentsEWStrategy(edgeWeighterProvider))
        {
        }
    }
}