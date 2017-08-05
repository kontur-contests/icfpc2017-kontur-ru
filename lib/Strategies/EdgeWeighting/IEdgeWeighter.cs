using lib.GraphImpl;

namespace lib.Strategies.EdgeWeighting
{
    public interface IEdgeWeighter
    {
        void Init(Graph graph, int[] claimedVertexIds);
        double EstimateWeight(Edge edge);
    }
}