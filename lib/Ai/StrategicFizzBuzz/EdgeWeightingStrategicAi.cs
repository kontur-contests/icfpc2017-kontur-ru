using System;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class EdgeWeightingStrategicAi : StrategicAi
    {
        protected EdgeWeightingStrategicAi(Func<SuperSettings, IEdgeWeighter> edgeWeighterProvider)
            : base(s => new EdgeWeightingStrategy(s.Map, s.PunterId, edgeWeighterProvider(s)))
        {
        }
    }
}