namespace lib.Scores
{
    public interface IScoreCalculator
    {
        long GetScore(int punter, Map map);
    }
}