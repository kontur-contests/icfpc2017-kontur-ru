using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.Structures;

namespace lib.Ai
{
    public class GreedyAiHelper
    {
        private readonly int punterId;
        private readonly MineDistCalculator mineDistCalulator;

        public GreedyAiHelper(int punterId, MineDistCalculator mineDistCalulator)
        {
            this.punterId = punterId;
            this.mineDistCalulator = mineDistCalulator;
        }

        public bool TryExtendAnything(Graph graph, out Move nextMove)
        {
            var calculator = new ConnectedCalculator(graph, punterId);
            var maxAddScore = long.MinValue;
            Edge bestEdge = null;
            foreach (var vertex in graph.Vertexes.Values)
            {
                foreach (var edge in vertex.Edges.Where(x => x.Owner == -1))
                {
                    var fromMines = calculator.GetConnectedMines(edge.From);
                    var toMines = calculator.GetConnectedMines(edge.To);
                    long addScore = Math.Max(
                        Calc(toMines, fromMines, edge.From),
                        Calc(fromMines, toMines, edge.To));
                    if (addScore > maxAddScore)
                    {
                        maxAddScore = addScore;
                        bestEdge = edge;
                    }
                }
            }
            if (bestEdge != null)
            {
                nextMove = Move.Claim(punterId, bestEdge.From, bestEdge.To);
                return true;
            }
            nextMove = Move.Pass(punterId);
            return false;
        }

        private long Calc(HashSet<int> mineIds, HashSet<int> diff, int vertexId)
        {
            return mineIds.Where(x => !diff.Contains(x)).Sum(
                mineId =>
                {
                    var dist = mineDistCalulator.GetDist(mineId, vertexId);
                    return (long)dist * dist;
                });
        }
    }
}