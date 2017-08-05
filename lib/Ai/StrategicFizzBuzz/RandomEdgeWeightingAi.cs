using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai.StrategicFizzBuzz
{
    public class RandomEdgeWeightingAi : EdgeWeightingStrategicAi
    {
        protected override IEdgeWeighter CreateEdgeWeighter(State state, IServices services)
        {
            return new RandomEdgeWeighter();
        }
    }
}