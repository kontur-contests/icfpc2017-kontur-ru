namespace lib.Structures
{
    public class Score
    {
        public int punter { get; set; }
        public long score { get; set; }

        public override string ToString()
        {
            return $"{nameof(punter)}: {punter}, {nameof(score)}: {score}";
        }
    }
}