using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class EdgeWeightingStrategicAi : StrategicAi
    {
        protected override IStrategy CreateStrategy(State state, IServices services)
        {
            return new EdgeWeightingStrategy(state.punter, CreateEdgeWeighter(state, services), services.Get<MineDistCalculator>(state));
        }
        
        protected abstract IEdgeWeighter CreateEdgeWeighter(State state, IServices services);
    }
}