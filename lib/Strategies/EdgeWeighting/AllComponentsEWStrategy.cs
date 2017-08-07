using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class AllComponentsEWStrategy : IStrategy, IMetaStrategy
    {
        public AllComponentsEWStrategy(IEdgeWeighter edgeWeighter, State state, IServices services, double optionPenaltyMultiplier = 1.0)
        {
            PunterId = state.punter;
            EdgeWeighter = edgeWeighter;
            State = state;
            Graph = services.Get<Graph>();
            MineDistCalulator = services.Get<MineDistCalculator>();
            ConnectedComponentsService = services.Get<ConnectedComponentsService>();
            OptionPenaltyMultiplier = optionPenaltyMultiplier;
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private State State { get; }
        private int PunterId { get; }
        private ConnectedComponentsService ConnectedComponentsService { get; }
        private Graph Graph { get; }
        private double OptionPenaltyMultiplier { get; }
        
        public List<TurnResult> NextTurns()
        {
            var connectedComponents = ConnectedComponentsService.For(PunterId);
            return connectedComponents.SelectMany(x => GetTurnsForComponents(Graph, connectedComponents, x)).ToList();
        }

        private List<TurnResult> GetTurnsForComponents(Graph graph, ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            EdgeWeighter.Init(connectedComponents, currentComponent);

            var vertices = currentComponent.Vertices
                .SelectMany(v => graph.Vertexes[v].Edges)
                .ToList();

            var claimVertices = vertices
                .Where(e => e.Owner == -1 && !AreConnected(currentComponent, e.From, e.To))
                .Select(e => new TurnResult
                {
                    Estimation = EdgeWeighter.EstimateWeight(e),
                    Move = AiMoveDecision.Claim(e, PunterId)
                });

            var optionVertices = !State.settings.options || State.map.OptionsLeft(State.punter) <= 0
                ? Enumerable.Empty<TurnResult>()
                : vertices
                    .Where(e => e.Owner != -1 && e.OptionOwner == -1 && !AreConnected(currentComponent, e.From, e.To))
                    .Select(e => new TurnResult
                    {
                        Estimation = EdgeWeighter.EstimateWeight(e) * OptionPenaltyMultiplier,
                        Move = AiMoveDecision.Option(e, PunterId)
                    }).ToList();

            return claimVertices.Concat(optionVertices)
                .Where(t => t.Estimation > 0)
                .ToList();
        }

        private bool AreConnected(ConnectedComponent currentComponent, int fromId, int toId)
        {
            return currentComponent.Vertices.Contains(fromId) && currentComponent.Vertices.Contains(toId);
        }
    }
}