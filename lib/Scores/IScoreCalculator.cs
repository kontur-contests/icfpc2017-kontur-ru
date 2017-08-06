using lib.Scores.Simple;
using lib.Structures;

namespace lib.Scores
{
    public class ScoreData
    {
        public long TotalScore => ScoreWithoutFutures - PossibleFuturesScore + 2 * GainedFuturesScore;
        public long ScoreWithoutFutures;
        public long PossibleFuturesScore;
        public long GainedFuturesScore;
        public int GainedFuturesCount;
        public int TotalFuturesCount;
    }

    public interface IScoreCalculator
    {
        long GetScore(int punter, Map map, Future[] futures);
        ScoreData GetScoreData(int punter, Map map, Future[] futures);
    }
}