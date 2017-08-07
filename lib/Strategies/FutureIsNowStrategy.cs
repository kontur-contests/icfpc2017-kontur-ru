using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;
using MoreLinq;

namespace lib.Strategies
{
    public class FutureIsNowStrategy : IStrategy
    {
        private readonly bool allowToUseOptions;
        private readonly State state;
        private readonly Graph graph;
        
        public FutureIsNowStrategy(bool allowToUseOptions, State state, IServices services)
        {
            this.allowToUseOptions = state.settings.options && allowToUseOptions && state.map.OptionsLeft(state.punter) > 0;
            this.state = state;
            graph = services.Get<Graph>();
        }

        public List<TurnResult> NextTurns()
        {
            var decision = TryGetNextMove();
            if (decision != null)
                return new List<TurnResult> { new TurnResult { Move = decision, Estimation = 1 } };
            return new List<TurnResult>();
        }

        private AiMoveDecision TryGetNextMove()
        {
            var sitesToDefend = state.aiSetupDecision.futures.SelectMany(f => new[] { f.source, f.target }).ToArray();
            var edge = new MovesSelector(allowToUseOptions, state.map, graph, sitesToDefend, state.punter).GetNeighbourToGo();
            if (edge != null)
                return AiMoveDecision.ClaimOrOption(edge, state.punter, allowToUseOptions, "futures cant wait!!1");
            return null;
        }
    }

    public class MovesSelector
    {
        private readonly bool haveFreeOption;
        private readonly Map map;
        private readonly Graph graph;
        private readonly int[] sitesToDefend;
        private readonly int punterId;

        public MovesSelector(bool haveFreeOption, Map map, Graph graph, int[] sitesToDefend, int punterId)
        {
            this.haveFreeOption = haveFreeOption;
            this.map = map;
            this.graph = graph;
            this.sitesToDefend = sitesToDefend;
            this.punterId = punterId;
        }

        public Edge GetNeighbourToGo()
        {
            List<HashSet<int>> components = sitesToDefend.Select(GetConnectedComponent)
                .DistinctBy(c => c.Min()).ToList();
            if (components.Count == 1) return null;
            var q =
                from component in components
                let neighbours = GetFreeNeighbours(component)
                select new { component, neighbours };

            var weakComponents = q.OrderBy(t => t.neighbours.Count - MinesToDefendablesRatio(t.component));

            foreach (var weakComponent in weakComponents)
            {
                var otherComponents = components.Where(c => c != weakComponent.component).ToHashSet();
                var otherOurSites = otherComponents.SelectMany(c => c).ToHashSet();
                var neighbourDistToOtherComp = GetDistFromNeighbourToDestinations_viaBfs(weakComponent.neighbours, otherOurSites);
                var candidateSites = neighbourDistToOtherComp.MaxListBy(r => -r.Value); // min dist

                if (candidateSites.Count > 0)
                {
                    int neighbourToGo = candidateSites.MaxBy(s => GetFanOutsCount(s.Key, weakComponent.component)).Key;
                    var weakComponentSiteIds = weakComponent.component;
                    return graph.Vertexes[neighbourToGo].Edges
                        .First(e => weakComponentSiteIds.Contains(e.To) && e.CanBeOwnedBy(punterId, haveFreeOption));
                }
            }

            return null;
        }

        private int GetFanOutsCount(int siteId, HashSet<int> component)
        {
            return graph.Vertexes[siteId].Edges.Count(e => e.IsFree && !component.Contains(e.To));
        }

        private double MinesToDefendablesRatio(IEnumerable<int> component)
        {
            double mines = 0;
            double defendables = 0;
            foreach (var vertex in component)
            {
                mines += map.Mines.Contains(vertex) ? 1 : 0;
                defendables += sitesToDefend.Contains(vertex) ? 1 : 0;
            }
            return mines / defendables;
        }

        private Dictionary<int, int> GetDistFromNeighbourToDestinations_viaBfs(List<int> neighbours, HashSet<int> destinationSiteIds)
        {
            var initiatorDistanceToDestination = new Dictionary<int, int>();
            var q = new Queue<int>();
            var initiator = new Dictionary<int, int>();
            var dist = new Dictionary<int, int>();
            foreach (int neighbour in neighbours)
            {
                initiator[neighbour] = neighbour;
                dist[neighbour] = 1;
                if (destinationSiteIds.Contains(neighbour))
                {
                    initiatorDistanceToDestination[neighbour] = 1;
                }
                else
                    q.Enqueue(neighbour);
            }
            while (q.Count > 0)
            {
                var currentId = q.Dequeue();
                var currentVertex = graph.Vertexes[currentId];
                int currentInitiator = initiator[currentId];
                int currentDistance = dist[currentId] + 1;
                foreach (var edge in currentVertex.Edges.Where(e => e.IsFree || e.IsOwnedBy(punterId)))
                {
                    if (destinationSiteIds.Contains(edge.To))
                    {
                        initiatorDistanceToDestination[currentInitiator] = 
                            Math.Min(
                                initiatorDistanceToDestination.GetOrDefault(currentInitiator, int.MaxValue), 
                                currentDistance);
                        break;
                    }
                    if (!initiator.ContainsKey(edge.To))
                    {
                        q.Enqueue(edge.To);
                        initiator[edge.To] = currentInitiator;
                        dist[edge.To] = currentDistance;
                    }
                }
            }
            return initiatorDistanceToDestination;
        }

        private List<int> GetFreeNeighbours(HashSet<int> component)
        {
            // используем опцион только для поиска соседей - кандидатов на ход, для следующих ребер мы не уверены, что опционы еще будут
            return component
                .SelectMany(s => graph.Vertexes[s].Edges.Where(e => e.CanBeOwnedBy(punterId, haveFreeOption) && !component.Contains(e.To))
                .Select(e => e.To))
                .Distinct().ToList();
        }

        private HashSet<int> GetConnectedComponent(int siteId)
        {
            var q = new Queue<int>();
            var used = new HashSet<int>();
            var component = new HashSet<int> { siteId };
            q.Enqueue(siteId);
            used.Add(siteId);
            while (q.Count > 0)
            {
                var currentId = q.Dequeue();
                var currentVertex = graph.Vertexes[currentId];
                foreach (var edge in currentVertex.Edges.Where(e => e.IsOwnedBy(punterId)))
                {
                    component.Add(edge.To);
                    component.Add(edge.From);
                    if (!used.Contains(edge.To))
                    {
                        q.Enqueue(edge.To);
                        used.Add(edge.To);
                    }
                }
            }
            return component;
        }
    }

}