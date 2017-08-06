using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;

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


        public List<TurnResult> Turn(Graph graph)
        {
            var connectedComponents = ConnectedComponent.GetComponents(graph, PunterId);
            FillMines(graph, connectedComponents);
            return connectedComponents.SelectMany(x => GetTurnsForComponents(graph, connectedComponents, x)).ToList();
        }

        private void FillMines(Graph graph, List<ConnectedComponent> connectedComponents)
        {
            var notConnectedMines = graph.Mines.Keys.Except(connectedComponents.SelectMany(x => x.Mines));
            foreach (var mine in notConnectedMines)
            {
                var connectedComponent = new ConnectedComponent(connectedComponents.Count);
                connectedComponent.Mines.Add(mine);
                connectedComponent.Vertices.Add(mine);
                connectedComponents.Add(connectedComponent);
            }
        }

        private List<TurnResult> GetTurnsForComponents(Graph graph, List<ConnectedComponent> connectedComponents, ConnectedComponent maxComponent)
        {
            EdgeWeighter.Init(graph, connectedComponents, maxComponent);
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