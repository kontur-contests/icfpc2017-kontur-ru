using lib.GraphImpl;
using lib.Structures;

namespace lib.Scores.Simple
{
    public class SimpleScoreCalculator : IScoreCalculator
    {
        public ScoreData GetScoreData(int punter, Map map, Future[] futures)
        {
            var scoreData = new ScoreData();
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
            scoreData.ScoreWithoutFutures = res;

            foreach (var future in futures)
            {
                var mines = minesCalc.GetConnectedMines(future.target);

                var dist = distCalc.GetDist(future.source, future.target);

                var futureScoreValue = dist * dist * dist;
                if (mines.Contains(future.source))
                {
                    scoreData.GainedFuturesScore += futureScoreValue;
                    scoreData.GainedFuturesCount++;
                }
                scoreData.PossibleFuturesScore += futureScoreValue;
                scoreData.TotalFuturesCount++;
            }

            return scoreData;
        }

        public long GetScore(int punter, Map map, Future[] futures)
        {
            return GetScoreData(punter, map, futures).TotalScore;
        }
    }
}