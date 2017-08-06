using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class LochKillerEdgeWeighter : IEdgeWeighter
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public LochKillerEdgeWeighter(State state, IServices services)
        {
            Graph = services.Get<Graph>();

            PuntersCount = state.punters;
        }

        private Graph Graph { get; }

        private int PuntersCount { get; }

        private Dictionary<Edge, double> EdgeWeights { get; set; }

        public void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            EdgeWeights = new Dictionary<Edge, double>();
            if (Graph.Vertexes.Count < 300)
                return;

            var nearMinesEdge = Graph.Mines.Keys
                .Select(mine => new {mine, edges = Graph.Vertexes[mine].Edges.ToList()})
                .Where(mine => mine.edges.Select(edge => edge.Owner).Distinct().Count() < PuntersCount + 1)
                .OrderBy(mine => Tuple.Create(mine.edges.Select(edge => edge.Owner).Distinct().Count(), Random.Value.Next()))
                .Where(mine => mine.edges.Count <= 100)
                .SelectMany(mine => mine.edges)
                .Where(edge => edge.Owner < 0);
            var weight = 0;
            foreach (var edge in nearMinesEdge.Reverse())
            {
                EdgeWeights[edge] = weight;
                weight += 1;
            }
        }

        public double EstimateWeight(Edge edge)
        {
            return EdgeWeights.GetOrDefault(edge, 0);
        }
    }
}