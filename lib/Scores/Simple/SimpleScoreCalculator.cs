using lib.GraphImpl;

namespace lib.Scores.Simple
{
    public class SimpleScoreCalculator : IScoreCalculator
    {
        public long GetScore(int punter, Map map)
        {
            var graph = new Graph(map);
            var distCalc = new MineDistCalculator(graph);
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
            return res;
        }
    }
}