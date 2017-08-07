using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Strategies
{
    public class LochDinicKillerStrategy2 : IStrategy
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private readonly Dictionary<Edge, double> edgesToBlock = new Dictionary<Edge, double>();
        private readonly ShortestPathGraphService SpGraphService;
        private readonly ConnectedComponentsService ConnectedComponentsService;

        private State state;
        private IServices services;

        public LochDinicKillerStrategy2(State state, IServices services)
        {
            this.state = state;
            this.services = services;

            Graph = services.Get<Graph>();
            ConnectedComponentsService = services.Get<ConnectedComponentsService>();
            SpGraphService = services.Get<ShortestPathGraphService>();
            PunterId = state.punter;
            PuntersCount = state.punters;
        }

        private Graph Graph { get; }
        private int PunterId { get; }
        private int PuntersCount { get; }

        public List<TurnResult> NextTurns()
        {
            Init();

            return Graph.Vertexes.Values
                .SelectMany(v => v.Edges)
                .Select(
                    edge => edge.IsFree ? new TurnResult
                    {
                        Estimation = EstimateWeight(edge),
                        Move = AiMoveDecision.Claim(edge, PunterId),
                    } : null)
                .Where(river => river != null && river.Estimation > 0)
                .ToList();
        }

        private void Init()
        {
            var connectedComponents = ConnectedComponentsService.For(PunterId);
            
            var shortWeighters = connectedComponents.Select(component =>
            {
                var c = new MaxVertextWeighter(1, state, services);
                c.Init(connectedComponents, component);
                return c;
            }).ToList();

            double getWeight(Edge edge)
            {
                return shortWeighters.Select(w => w.EstimateWeight(edge)).Sum();
            }

            var maxCount = 10;
            edgesToBlock.Clear();

            var mineToSave = Graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => !edge.IsOwnedBy(PunterId)))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.IsFree) < PuntersCount)
                .Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => Random.Value.Next()).FirstOrDefault(edge => edge.IsFree);
                if (edgeToSave != null)
                    edgesToBlock[edgeToSave] = 10;
            }

            var bannedMines = Graph.Mines
                .Where(mine => mine.Value.Edges.SelectMany(edge => edge.GetOwners()).Distinct().Count() == PuntersCount)
                .Select(mine => mine.Key)
                .ToHashSet();

            var vToComp = connectedComponents.SelectMany(c => c.Vertices.Select(v => new {v, c}))
                .ToDictionary(p => p.v, p => p.c);

            int ChooseRandom(double[] sums)
            {
                var r = Random.Value.NextDouble()*sums[sums.Length - 1];

                var sr = Array.BinarySearch(sums, r);
                return sr < 0 ? Math.Min(sums.Length - 1, ~sr) : sr;
            }

            var interestingPoints = Graph.Vertexes.SelectMany(v => v.Value.Edges)
                .Select(edge => new {p = edge.From, w = getWeight(edge)}).ToList();
            var sum = 0.0;
            var summes = interestingPoints.Select(p => sum += p.w).ToArray();

            var mines = Graph.Mines.Where(mine => mine.Value.Edges.Any(edge => edge.IsFree)).ToList();
            if (mines.Count >= 2)
            {
                for (var i = 0; i < 20; i++)
                {
                    var mine1 = mines[Random.Value.Next(mines.Count)];
                    var p = interestingPoints[ChooseRandom(summes)];
                    while (p.p== mine1.Key) p = interestingPoints[ChooseRandom(summes)];

                    // Чтобы не ходить по своим рёбрам в динице. Таким образом, длинные мосты блочаться один раз.
                    const int nonExistentPunterId = 1000000;
                    var dinic = new Dinic2(Graph, nonExistentPunterId, mine1.Key, p.p, out var flow);
                    if (flow == 0)
                        continue;
                    if (flow > maxCount)
                        continue;

                    var compSize = GetOpenComponentSize(mine1.Key);
                    var mincut = dinic.GetOptimalMinCut(compSize,  out int size);

                    if (size > compSize / 2)
                        size = compSize - size;
                
                    foreach (var edge in mincut.Select(edge1 => edge1))
                    {
                        if (bannedMines.Contains(edge.From) || bannedMines.Contains(edge.To))
                            continue;
                        edgesToBlock[edge] = edgesToBlock.GetOrDefault(edge, 0) + size*1.0/flow;
                    }
                }
                
            }
            edgesToBlock.Keys.ToList().ForEach(
                key =>
                {
                    if (edgesToBlock[key] < Graph.Vertexes.Count / 50.0 || edgesToBlock[key] < 4)
                        edgesToBlock.Remove(key);
                });
        }

        public int GetOpenComponentSize(int vertex)
        {
            var queue = new Queue<int>();
            queue.Enqueue(vertex);
            var used = new HashSet<int>{vertex};

            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                var vtx = Graph.Vertexes[n];
                foreach (var edge in vtx.Edges)
                {
                    if (edge.IsOwnedBy(PunterId) || used.Contains(edge.To))
                        continue;
                    used.Add(edge.To);
                    queue.Enqueue(edge.To);
                }
            }
            return used.Count;
        }

        private double EstimateWeight(Edge edge)
        {
            return edgesToBlock.GetOrDefault(edge, 0);
        }
    }
}