using System.Collections.Generic;
using lib.GraphImpl;

namespace lib.Strategies.EdgeWeighting
{
    public interface IEdgeWeighter
    {
        void Init(Graph graph, List<ConnectedComponent> connectedComponents, ConnectedComponent currentComponent);
        double EstimateWeight(Edge edge);
    }
}