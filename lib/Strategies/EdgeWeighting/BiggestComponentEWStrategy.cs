using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using MoreLinq;

namespace lib.Strategies.EdgeWeighting
{
    public class BiggestComponentEWStrategy : IStrategy
    {
        public BiggestComponentEWStrategy(IEdgeWeighter edgeWeighter, State state, IServices services)
        {
            PunterId = state.punter;
            EdgeWeighter = edgeWeighter;
            MineDistCalulator = services.Get<MineDistCalculator>(state);
            ConnectedComponentsService = services.Get<ConnectedComponentsService>(state);
            GraphService = services.Get<GraphService>(state);
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private int PunterId { get; }
        private ConnectedComponentsService ConnectedComponentsService { get; }
        private GraphService GraphService { get; }

        public List<TurnResult> NextTurns()
        {
            var graph = GraphService.Graph;
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

            var connectedComponents = ConnectedComponentsService.For(PunterId);
            var maxComponent = connectedComponents.MaxBy(comp => comp.Vertices.Count);
            EdgeWeighter.Init(connectedComponents, maxComponent);
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