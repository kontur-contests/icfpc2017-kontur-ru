using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class AllComponentsEWStrategy : IStrategy
    {
        public AllComponentsEWStrategy(int punterId, IEdgeWeighter edgeWeighter, MineDistCalculator mineDistCalculator)
        {
            PunterId = punterId;
            EdgeWeighter = edgeWeighter;
            MineDistCalulator = mineDistCalculator;
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private int PunterId { get; }


        public List<TurnResult> Turn(State state, IServices services)
        {
            var graph = services.Get<Graph>();
            var connectedComponents = ConnectedComponent.GetComponents(graph, PunterId);
            FillMines(graph, connectedComponents);
            return connectedComponents.SelectMany(x => GetTurnsForComponents(state, services, connectedComponents, x)).ToList();
        }

        private void FillMines(Graph graph, List<ConnectedComponent> connectedComponents)
        {
            var notConnectedMines = graph.Mines.Keys.Except(connectedComponents.SelectMany(x => x.Mines));
            foreach (var mine in notConnectedMines)
            {
                var connectedComponent = new ConnectedComponent(connectedComponents.Count, PunterId);
                connectedComponent.Mines.Add(mine);
                connectedComponent.Vertices.Add(mine);
                connectedComponents.Add(connectedComponent);
            }
        }

        private List<TurnResult> GetTurnsForComponents(State state, IServices services, List<ConnectedComponent> connectedComponents, ConnectedComponent maxComponent)
        {
            var graph = services.Get<Graph>();
            EdgeWeighter.Init(state, services, connectedComponents, maxComponent);
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