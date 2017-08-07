using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.Ai;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.Scores.Simple;
using lib.StateImpl;
using lib.Structures;
using NUnit.Framework;

namespace lib.Strategies
{
    namespace lib.Strategies
    {
        public class AntiLochDinicStrategy : IStrategy
        {
            private static readonly ThreadLocal<Random> Random =
                new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));


            private readonly Dictionary<Edge, double> edgesToBlock = new Dictionary<Edge, double>();

            public AntiLochDinicStrategy(State state, IServices services, bool options = false)
            {
                this.options = options;
                Graph = services.Get<Graph>();
                PunterId = state.punter;

                this.options &= state.settings.options;
                this.options &=
                    state.map.OptionsUsed.GetOrDefaultNoSideEffects(PunterId, 0) >= state.map.Mines.Length;
            }


            private readonly bool options;
            private Graph Graph { get; }
            private int PunterId { get; }

            public List<TurnResult> NextTurns()
            {
                Init();

                return Graph.Vertexes.Values
                    .SelectMany(v => v.Edges)
                    .Select(
                        edge => new TurnResult
                        {
                            Estimation = EstimateWeight(edge),
                            Move = edge.Owner == -1
                                ? AiMoveDecision.Claim(PunterId, edge.River.Source, edge.River.Target)
                                : AiMoveDecision.Option(PunterId, edge.River.Source, edge.River.Target)
                        })
                    .Where(river => river.Estimation > 0)
                    .ToList();
            }

            private void Init()
            {
                var mines = Graph.Mines.Select(x => x.Key).ToList();
                var comps = mines.ToDictionary(x => x, GetConnectedComponent);

                foreach (var mine1 in mines)
                {
                    foreach (var mine2 in mines)
                    {
                        if (mine1 >= mine2)
                            continue;

                        var dinic = new Dinic(Graph, PunterId, mine1, mine2, out var flow);

                        //TODO: учесть размеры компонент
                        if (flow == 0 && TryUseOption(mine1, mine2, comps, out var edge2))
                            edgesToBlock[edge2] = 2;

                        if (flow != 1)
                            continue;

                        foreach (var edge in dinic.GetMinCut().Select(edge1 => edge1))
                            edgesToBlock[edge] = 1;
                    }
                }
            }

            private bool TryUseOption(int start, int finish, Dictionary<int, HashSet<int>> comps, out Edge result)
            {
                result = null;

                if (!options)
                    return false;

                var dist = new Dictionary<int, int>();
                var parrent = new Dictionary<int, int>();

                int weightFree = 1;
                int weightMy = 0;
                int weightEnemy = 10; //TODO:

                var queues = new Queue<int>[weightEnemy + 1];
                for (int i = 0; i < queues.Length; i++)
                    queues[i] = new Queue<int>();

                int ptr = 0;

                dist[start] = 0;
                parrent[start] = -1;
                queues[ptr].Enqueue(start);
                int inQueue = 1;

                while (inQueue > 0)
                {
                    while (queues[ptr].Count == 0)
                        ptr = (ptr + 1) % queues.Length;

                    int v = queues[ptr].Dequeue();
                    inQueue--;

                    foreach (var edge in Graph.Vertexes[v].Edges)
                    {
                        var weight = -1;
                        if (edge.IsOwnedBy(PunterId))
                            weight = weightMy;
                        if (edge.Owner != -1 && edge.Owner != PunterId && edge.OptionOwner == -1)
                            weight = weightEnemy;
                        if (edge.Owner == -1)
                            weight = weightFree;

                        int u = edge.To;
                        int du = dist[v] + weight;

                        if (dist.ContainsKey(u) && dist[u] <= du)
                            continue;

                        dist[u] = du;
                        parrent[u] = v;
                        queues[(ptr + du) % queues.Length].Enqueue(u);
                        inQueue++;
                    }
                }

                if (!parrent.ContainsKey(finish))
                    return false;
                //TODO:
                if (dist[finish] >= 20)
                    return false;

                while (true)
                {
                    int df = dist[finish];
                    int finish2 = parrent[finish];
                    int df2 = dist[finish];
                    if (df - df2 == weightEnemy)
                    {
                        result = new Edge(finish, finish2, -2, -1);
                        return true;
                    }

                    finish = finish2;
                }
            }

            private HashSet<int> GetConnectedComponent(int siteId)
            {
                var q = new Queue<int>();
                var used = new HashSet<int>();
                q.Enqueue(siteId);
                used.Add(siteId);
                while (q.Count > 0)
                {
                    var currentId = q.Dequeue();
                    var currentVertex = Graph.Vertexes[currentId];
                    foreach (var edge in currentVertex.Edges.Where(e => e.IsOwnedBy(PunterId)))
                    {
                        if (!used.Contains(edge.To))
                        {
                            q.Enqueue(edge.To);
                            used.Add(edge.To);
                        }
                    }
                }
                return used;
            }


            private double EstimateWeight(Edge edge)
            {
                return edgesToBlock.GetOrDefault(edge, 0);
            }
        }
    }

    [TestFixture]
    public class AntiAi_Run
    {
        [Test]
        public void Test1()
        {
            var gamers = new List<IAi> { new LochDinicKillerAi(), new LochDinicKillerAi(), new LochDinicKillerAi(), new OptAntiLochDinicKillerAi() };
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());

            var results = gameSimulator.SimulateGame(
                gamers, MapLoader.LoadMapByName("lambda.json").Map, new Settings());

            foreach (var gameSimulationResult in results)
                Console.Out.WriteLine(
                    "gameSimulationResult = {0}:{1}", gameSimulationResult.Gamer.Name, gameSimulationResult.Score);
        }
    }
}