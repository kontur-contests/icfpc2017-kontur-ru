using System;
using System.Threading;
using lib.GraphImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class RandomEdgeWeighter : IEdgeWeighter
    {
        private static ThreadLocal<Random> Random { get; } = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public void Init(Graph graph, int[] claimedVertexIds, int ownerId)
        {
        }

        public double EstimateWeight(Edge edge)
        {
            return Random.Value.NextDouble();
        }
    }
}