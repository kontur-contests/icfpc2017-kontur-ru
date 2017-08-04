namespace lib.Scores
{
    public interface IScoreCalculator
    {
        int GetScore(int punter, Map map);
    }
}