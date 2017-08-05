using System.Linq;

namespace lib.Structures
{
    public class Moves
    {
        public Move[] moves;

        public override string ToString()
        {
            return $"{nameof(moves)}: {(moves == null ? "" : string.Join("; ", moves.Select(x => x.ToString())))}";
        }
    }
}