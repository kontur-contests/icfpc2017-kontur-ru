using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class MineGuardianEdgeWeighter : IEdgeWeighter
    {
        public MineGuardianEdgeWeighter(State state, IServices services, int outEdgesWarningLevel = 3, int guardedComponentSizeThreshold = 10)
        {
            Graph = services.Get<Graph>();
            OutEdgesWarningLevel = outEdgesWarningLevel;
            GuardedComponentSizeThreshold = guardedComponentSizeThreshold;
            PunterId = state.punter;
        }

        private Graph Graph { get; }
        private int OutEdgesWarningLevel { get; }
        private int GuardedComponentSizeThreshold { get; }
        private int PunterId { get; }

        private Dictionary<Edge, double> EdgeWeights { get; set; }

        public void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            EdgeWeights = new Dictionary<Edge, double>();
            if (TryGuardSingleMine(currentComponent))
                return;
            TryGuardSmallComponent(currentComponent);
        }

        public double EstimateWeight(Edge edge)
        {
            return EdgeWeights.GetOrDefault(edge, 0);
        }

        private bool TryGuardSmallComponent(ConnectedComponent currentComponent)
        {
            if (currentComponent.Vertices.Count > GuardedComponentSizeThreshold)
                return false;
            var componentEdges = currentComponent.Vertices.SelectMany(v => Graph.Vertexes[v].Edges).ToList();
            return TryGuardEdges(componentEdges);
        }

        private bool TryGuardSingleMine(ConnectedComponent currentComponent)
        {
            if (currentComponent.Mines.Count != 1)
                return false;
            var mine = currentComponent.Mines.Single();
            var mineEdges = Graph.Vertexes[mine].Edges;
            return TryGuardEdges(mineEdges);
        }

        private bool TryGuardEdges(List<Edge> outEdges)
        {
            var outEdgesCount = outEdges.Count(e => e.IsFree || e.Owner == PunterId);
            var freeEdgesCount = outEdges.Count(e => e.IsFree);
            if (outEdgesCount >= OutEdgesWarningLevel || freeEdgesCount == 0)
                return false;
            foreach (var outEdge in outEdges.Where(e => e.IsFree))
                EdgeWeights[outEdge] = 1.0 / outEdgesCount;
            return true;
        }
    }
}