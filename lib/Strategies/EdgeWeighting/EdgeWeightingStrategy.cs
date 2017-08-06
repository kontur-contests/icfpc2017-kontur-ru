using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using MoreLinq;

namespace lib.Strategies.EdgeWeighting
{
    public class EdgeWeightingStrategy : IStrategy
    {
        public EdgeWeightingStrategy(int punterId, IEdgeWeighter edgeWeighter, MineDistCalculator mineDistCalculator)
        {
            PunterId = punterId;
            EdgeWeighter = edgeWeighter;
            MineDistCalulator = mineDistCalculator;
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private int PunterId { get; }


        public List<TurnResult> Turn(Graph graph)
        {
            var claimedVertexes = graph.Vertexes.Values
                .SelectMany(x => x.Edges)
                .Where(edge => edge.Owner == PunterId)
                .SelectMany(edge => new[] {edge.From, edge.To})
                .Distinct()
                .ToArray();

            if (claimedVertexes.Length == 0)
                return graph.Mines.Values
                    .SelectMany(v => v.Edges)
                    .Where(e => e.Owner == -1)
                    .Select(
                        e => new TurnResult
                        {
                            Estimation = 1,
                            River = e.River
                        })
                    .ToList();

            var connectedComponents = ConnectedComponent.GetComponents(graph, PunterId);
            EdgeWeighter.Init(graph, connectedComponents);
            var maxComponent = connectedComponents.MaxBy(comp => comp.Vertices.Count);
            return maxComponent.Vertices
                .SelectMany(v => graph.Vertexes[v].Edges)
                .Where(e => e.Owner == -1)
                .Select(
                    e => new TurnResult
                    {
                        Estimation = EdgeWeighter.EstimateWeight(e),
                        River = e.River
                    })
                .ToList();
        }
    }
}