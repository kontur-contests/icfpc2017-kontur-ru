using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.Strategies;
using lib.Structures;
using lib.viz;
using lib.viz.Detalization;
using MoreLinq;
using NUnit.Framework;

namespace lib.Ai
{
    [ShoulNotRunOnline]
    public class FutureIsNow : IAi
    {
        private List<int> path;
        private int punterId;
        private Future[] futures;
        public string Name => "Futurer";
        public string Version => "0";
        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            var graph = new Graph(map);
            var mineDists = new MineDistCalculator(graph);
            var length = 5*puntersCount;
            path = new PathSelector(map, mineDists, length).SelectPath();
            futures = new FuturesPositioner(map, graph, path, mineDists).GetFutures();
            return futures;
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var sitesToDefend = futures.SelectMany(f => new[]{f.source, f.target}).ToArray();
            var edge = new MovesSelector(map,new Graph(map), sitesToDefend, punterId).GetNeighbourToGo();
            if (edge == null)
            {
                return new GameplayOut { pass = new PassMove() };
            }
            var claimMove = new ClaimMove { punter = punterId, source = edge.From, target = edge.To };
            return new GameplayOut { claim = claimMove };
        }

        public string SerializeGameState()
        {
            throw new System.NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new System.NotImplementedException();
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
            var weakComponent = q.MinBy(t => t.neighbours.Count);
            var otherComponents = components.Where(c => c != weakComponent.c).ToHashSet();
            var res = Bfs(weakComponent.neighbours, otherComponents.SelectMany(c=>c).ToHashSet());
            var candidateSites = res.MaxListBy(r => -r.Value);
            if (candidateSites.Count > 0)
            {
                int neighbourToGo = candidateSites[0].Key;
                var weakComponentSiteIds = weakComponent.c;
                return graph.Vertexes[neighbourToGo].Edges.First(e => weakComponentSiteIds.Contains(e.To));
            }
            return null;
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