using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public class MaxReachableVertexWeightAi : EdgeWeightingStrategicAi
    {
        protected override IEdgeWeighter CreateEdgeWeighter(State state, IServices services)
        {
            return new MaxVertextWeighterWithConnectedComponents(state.map, services.Get<MineDistCalculator>(state));
        }
    }
}