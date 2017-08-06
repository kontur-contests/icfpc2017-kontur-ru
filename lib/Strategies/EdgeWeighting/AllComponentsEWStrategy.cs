using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class AllComponentsEWStrategy : IStrategy, IMetaStrategy
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
            return connectedComponents.SelectMany(x => GetTurnsForComponents(Graph, connectedComponents, x)).ToList();
        }

        private List<TurnResult> GetTurnsForComponents(Graph graph, ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            EdgeWeighter.Init(connectedComponents, currentComponent);
            return currentComponent.Vertices
                .SelectMany(v => graph.Vertexes[v].Edges)
                .Where(e => e.Owner == -1 && !AreConnected(currentComponent, e.From, e.To)) //Options
                .Select(
                    e => new TurnResult
                    {
                        Estimation = EdgeWeighter.EstimateWeight(e),
                        Move = AiMoveDecision.Claim(PunterId, e.River.Source, e.River.Target)
                    })
                .Where(t => t.Estimation > 0)
                .ToList();
        }

        private bool AreConnected(ConnectedComponent currentComponent, int fromId, int toId)
        {
            return currentComponent.Vertices.Contains(fromId) && currentComponent.Vertices.Contains(toId);
        }
    }
}