using lib.GraphImpl;

namespace lib.Strategies.EdgeWeighting
{
    public interface IEdgeWeighter
    {
        void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent);
        double EstimateWeight(Edge edge);
    }
}