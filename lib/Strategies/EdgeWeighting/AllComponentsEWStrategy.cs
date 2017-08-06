using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class AllComponentsEWStrategy : IStrategy
    {
        public AllComponentsEWStrategy(IEdgeWeighter edgeWeighter, State state, IServices services)
        {
            PunterId = state.punter;
            EdgeWeighter = edgeWeighter;
            GraphService = services.Get<GraphService>(state);
            MineDistCalulator = services.Get<MineDistCalculator>(state);
            ConnectedComponentsService = services.Get<ConnectedComponentsService>(state);
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private int PunterId { get; }
        private ConnectedComponentsService ConnectedComponentsService { get; }
        private GraphService GraphService { get; }


        public List<TurnResult> NextTurns()
        {
            var graph = GraphService.Graph;
            var allComponents = GetAllComponents(graph).ToArray();
            return allComponents.SelectMany(x => GetTurnsForComponents(graph, allComponents, x)).ToList();
        }

        private IEnumerable<ConnectedComponent> GetAllComponents(Graph graph)
        {
            var connectedComponents = ConnectedComponentsService.For(PunterId);
            foreach (var connectedComponent in connectedComponents)
                yield return connectedComponent;
            var notConnectedMines = graph.Mines.Keys.Except(connectedComponents.SelectMany(x => x.Mines));
            foreach (var mine in notConnectedMines)
            {
                var connectedComponent = new ConnectedComponent(connectedComponents.Length, PunterId);
                connectedComponent.Mines.Add(mine);
                connectedComponent.Vertices.Add(mine);
                yield return connectedComponent;
            }
        }

        private List<TurnResult> GetTurnsForComponents(Graph graph, ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            EdgeWeighter.Init(connectedComponents, currentComponent);
            return currentComponent.Vertices
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