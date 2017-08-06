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
            Graph = services.Get<Graph>();
            MineDistCalulator = services.Get<MineDistCalculator>();
            ConnectedComponentsService = services.Get<ConnectedComponentsService>();
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private int PunterId { get; }
        private ConnectedComponentsService ConnectedComponentsService { get; }
        private Graph Graph { get; }


        public List<TurnResult> NextTurns()
        {
            var connectedComponents = ConnectedComponentsService.For(PunterId);
            var allComponents = GetAllComponents(Graph, connectedComponents).ToArray();
            return allComponents.SelectMany(x => GetTurnsForComponents(Graph, connectedComponents, x)).ToList();
        }

        private IEnumerable<ConnectedComponent> GetAllComponents(Graph graph, ConnectedComponent[] connectedComponents)
        {
            foreach (var connectedComponent in connectedComponents)
                yield return connectedComponent;
            var notConnectedMines = graph.Mines.Keys.Except(connectedComponents.SelectMany(x => x.Mines));
            foreach (var mine in notConnectedMines)
            {
                var connectedComponent = new ConnectedComponent(-1, PunterId);
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
                .Where(e => e.Owner == -1 && !AreConnected(currentComponent, e.From, e.To))
                .Select(
                    e => new TurnResult
                    {
                        Estimation = EdgeWeighter.EstimateWeight(e),
                        River = e.River
                    })
                .ToList();
        }

        private bool AreConnected(ConnectedComponent currentComponent, int fromId, int toId)
        {
            return currentComponent.Vertices.Contains(fromId) && currentComponent.Vertices.Contains(toId);
        }
    }
}