using lib.GraphImpl;
using lib.Structures;

namespace lib.Scores.Simple
{
    public class SimpleScoreCalculator : IScoreCalculator
    {
        public long GetScore(int punter, Map map, Future[] futures)
        {
            var graph = new Graph(map);
            var distCalc = new MineDistCalculator.Impl(graph);
            var minesCalc = new ConnectedCalculator(graph, punter);

            long res = 0;
            foreach (var vertex in graph.Vertexes)
            {
                var mines = minesCalc.GetConnectedMines(vertex.Key);
                foreach (var mine in mines)
                {
                    long dist = distCalc.GetDist(mine, vertex.Key);
                    res += dist * dist;
                }
            }

            foreach (var future in futures)
            {
                var mines = minesCalc.GetConnectedMines(future.target);

                var dist = distCalc.GetDist(future.source, future.target);

                var futureScoreValue = dist * dist * dist;

                var futureScore = mines.Contains(future.source)
                    ? futureScoreValue
                    : -futureScoreValue;

                res += futureScore;
            }

            return res;
        }
    }
}