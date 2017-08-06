using System.Collections.Generic;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public interface IEdgeWeighter
    {
        void Init(State state, IServices services, List<ConnectedComponent> connectedComponents, ConnectedComponent currentComponent);
        double EstimateWeight(Edge edge);
    }
}