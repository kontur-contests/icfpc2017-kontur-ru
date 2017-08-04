using System.Collections.Generic;

namespace lib
{
    public class GameState
    {
        public Map CurrentMap { get; }
        public int CurrentPunter { get; }
        public List<Move> PreviousMoves { get; }
    }
}