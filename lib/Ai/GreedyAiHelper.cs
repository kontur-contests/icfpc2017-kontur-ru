using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.Structures;

namespace lib.Ai
{
    public static class GreedyAiHelper
    {
        public static bool TryExtendAnything(int punterId, Graph graph, ConnectedCalculator calculator, MineDistCalculator mineDistCalulator, out Move nextMove)
        {
            var maxAddScore = long.MinValue;
            Edge bestEdge = null;
            foreach (var vertex in graph.Vertexes.Values)
            {
                foreach (var edge in vertex.Edges.Where(x => x.Owner == -1))
                {
                    var fromMines = calculator.GetConnectedMines(edge.From);
                    var toMines = calculator.GetConnectedMines(edge.To);
                    long addScore = Math.Max(
                        Calc(mineDistCalulator, toMines, fromMines, edge.From),
                        Calc(mineDistCalulator, fromMines, toMines, edge.To));
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

        private static long Calc(MineDistCalculator mineDistCalulator, HashSet<int> mineIds, HashSet<int> diff, int vertexId)
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