using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.Strategies;
using lib.Structures;

namespace lib.Ai
{
    public class LochDinicKiller : IAi
    {
        private IAi Base = new MaxReachableVertexWithConnectedComponentsWeightAi();

        private int punterId;
        private int puntersCount;

        private Random rand = new Random();
        public string Name => nameof(LochDinicKiller);
        public string Version => "0.2";


        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            this.puntersCount = puntersCount;
            return Base.StartRound(punterId, puntersCount, map, settings);
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var graph = new Graph(map);

            int maxCount = 10;
            List<Edge> edgesToBlock = new List<Edge>();

            var mineToSave = graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => edge.Owner != punterId))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.Owner < 0) < puntersCount).Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => rand.Next()).First(edge => edge.Owner < 0);
                return Move.Claim(punterId, edgeToSave.From, edgeToSave.To);
            }

            var bannedMines = graph.Mines
                .Where(mine => mine.Value.Edges.Select(edge => edge.Owner).Distinct().Count() == puntersCount + 1)
                .Select(mine => mine.Key)
                .ToHashSet();

            var mines = graph.Mines.Where(mine => mine.Value.Edges.Any(edge => edge.Owner < 0)).ToList();
            for (int i = 0; i < Math.Min(10, mines.Count*(mines.Count - 1)); i++)
            {
                var mine1 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];
                var mine2 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];
                while(mine2.Key == mine1.Key) mine2 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];

                var dinic = new Dinic(graph, punterId, mine1.Key, mine2.Key, out var flow);
                if (flow == 0)
                    continue;
                if (flow > maxCount)
                    continue;
                edgesToBlock.AddRange(dinic.GetMinCut().Where(edge => !bannedMines.Contains(edge.From)));
            }

            edgesToBlock = edgesToBlock.Distinct().ToList();

            if (edgesToBlock.Count == 0)
                return Base.GetNextMove(prevMoves, map);
            var choosenEdge = edgesToBlock[Math.Min(edgesToBlock.Count - 1, rand.Next(edgesToBlock.Count))]; 
            return Move.Claim(punterId, choosenEdge.From, choosenEdge.To);
        }

        public string SerializeGameState()
        {
            return Base.SerializeGameState();
        }

        public void DeserializeGameState(string gameState)
        {
            Base.DeserializeGameState(gameState);
        }
    }
}