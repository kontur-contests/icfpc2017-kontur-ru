using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.viz;
using MoreLinq;

namespace lib.Ai
{
    public class FutureIsNow : IAi
    {
        public string Name => "Futurer";
        public string Version => "0";

        public AiSetupDecision Setup(State state, IServices services)
        {
            var graph = services.Get<GraphService>(state).Graph;
            var mineDists = services.Get<MineDistCalculator>(state);
            var length = 5 * state.punters;
            var path = new PathSelector(state.map, mineDists.impl, length).SelectPath();
            var futures = new FuturesPositioner(state.map, graph, path, mineDists.impl).GetFutures();
            return AiSetupDecision.Create(futures);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var graph = services.Get<GraphService>(state).Graph;
            var sitesToDefend = state.aiSetupDecision.futures.SelectMany(f => new[]{f.source, f.target}).ToArray();
            var edge = new MovesSelector(state.map, graph, sitesToDefend, state.punter).GetNeighbourToGo();
            if (edge == null)
            {
                state.ccm = state.ccm ?? FillCcmState(state, graph);
                return new ConnectClosestMinesAi().GetNextMove(state, services);
            }
            return AiMoveDecision.Claim(state.punter, edge.From, edge.To);
        }

        private static ConnectClosestMinesAi.AiState FillCcmState(State state, Graph graph)
        {
            var myMines = graph.Mines
                .Where(e => e.Value.Edges.Any(g => g.Owner == state.punter))
                .Select(e => e.Key)
                .ToHashSet();

            return new ConnectClosestMinesAi.AiState {stage = 0, myMines = myMines};
        }
    }


    public class MovesSelector
    {
        private readonly Map map;
        private readonly Graph graph;
        private readonly int[] sitesToDefend;
        private readonly int punterId;

        public MovesSelector(Map map, Graph graph, int[] sitesToDefend, int punterId)
        {
            this.map = map;
            this.graph = graph;
            this.sitesToDefend = sitesToDefend;
            this.punterId = punterId;
        }

        public Edge GetNeighbourToGo()
        {
            var components = sitesToDefend.Select(s => GetConnectedComponent(s).ToList())
                .DistinctBy(c => c.Min()).ToList();
            if (components.Count == 1) return null;
            var q =
                from c in components
                let neighbours = GetFreeNeighbours(c)
                select new{c, neighbours};

            var weakComponents = q.OrderBy(t => t.neighbours.Count - MinesToDefendablesRatio(t.c));

            foreach (var weakComponent in weakComponents)
            {
                var otherComponents = components.Where(c => c != weakComponent.c).ToHashSet();
                var res = Bfs(weakComponent.neighbours, otherComponents.SelectMany(c => c).ToHashSet());
                var candidateSites = res.MaxListBy(r => -r.Value);

                if (candidateSites.Count > 0)
                {
                    int neighbourToGo = candidateSites[0].Key;
                    var weakComponentSiteIds = weakComponent.c;
                    return graph.Vertexes[neighbourToGo].Edges.First(e => weakComponentSiteIds.Contains(e.To));
                }
            }

            return null;
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

        private Dictionary<int, int> Bfs(List<int> neighbours, HashSet<int> destinationSiteIds)
        {
            var distanceToDestination = new Dictionary<int, int>();
            var q = new Queue<int>();
            var initiator = new Dictionary<int, int>();
            var dist = new Dictionary<int, int>();
            foreach (int neighbour in neighbours)
            {
                initiator[neighbour] = neighbour;
                dist[neighbour] = 1;
                if (destinationSiteIds.Contains(neighbour))
                {
                    distanceToDestination[neighbour] = 1;
                }
                else
                    q.Enqueue(neighbour);
            }
            while (q.Count > 0)
            {
                var currentId = q.Dequeue();
                var currentVertex = graph.Vertexes[currentId];
                foreach (var edge in currentVertex.Edges.Where(e => e.Owner == -1))
                {
                    int currentInitiator = initiator[currentId];
                    if (destinationSiteIds.Contains(edge.To))
                    {
                        distanceToDestination[currentInitiator] = dist[currentId]+1;
                        break;
                    }
                    if (!initiator.ContainsKey(edge.To))
                    {
                        q.Enqueue(edge.To);
                        initiator[edge.To] = currentInitiator;
                        dist[edge.To] = dist[currentId] + 1;
                    }
                }
            }
            return distanceToDestination;
        }

        private List<int> GetFreeNeighbours(List<int> component)
        {
            return component
                .SelectMany(s => graph.Vertexes[s].Edges.Where(e => e.Owner == -1).Select(e => e.To))
                .Distinct().ToList();
        }

        private HashSet<int> GetConnectedComponent(int siteId)
        {
            var q = new Queue<int>();
            var used = new HashSet<int>();
            var component = new HashSet<int>{siteId};
            q.Enqueue(siteId);
            used.Add(siteId);
            while(q.Count > 0)
            {
                var currentId = q.Dequeue();
                var currentVertex = graph.Vertexes[currentId];
                foreach (var edge in currentVertex.Edges.Where(e => e.Owner == punterId))
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