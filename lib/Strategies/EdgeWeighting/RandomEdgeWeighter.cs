using System;
using System.Collections.Generic;
using System.Threading;
using lib.GraphImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class RandomEdgeWeighter : IEdgeWeighter
    {
        private static ThreadLocal<Random> Random { get; } = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        public ConnectedComponent CurrentComponent => null;

        public void Init(Graph graph, List<ConnectedComponent> connectedComponents)
        {
        }

        public double EstimateWeight(Edge edge)
        {
            return Random.Value.NextDouble();
        }
    }
}