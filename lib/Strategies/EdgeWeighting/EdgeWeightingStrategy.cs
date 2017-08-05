using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class EdgeWeightingStrategy : IStrategy
    {
        public EdgeWeightingStrategy(Map map, int punterId, IEdgeWeighter edgeWeighter)
        {
            PunterId = punterId;
            EdgeWeighter = edgeWeighter;
            MineDistCalulator = new MineDistCalculator(new Graph(map));
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
                claimedVertexes = graph.Mines.Keys.ToArray();

            EdgeWeighter.Init(graph, claimedVertexes);
            return claimedVertexes.SelectMany(v => graph.Vertexes[v].Edges)
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