using System.Linq;

namespace lib.Structures
{
    public class StopIn
    {
        public Move[] moves;
        public Score[] scores;

        public override string ToString()
        {
            return $"{nameof(moves)}: {(moves == null ? "" : string.Join("; ", moves.Select(x => x.ToString())))}, {nameof(scores)}: {(scores == null ? "" : string.Join("; ", scores.Select(x => x.ToString())))}";
        }
    }
}