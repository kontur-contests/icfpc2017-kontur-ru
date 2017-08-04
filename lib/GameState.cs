using System.Collections.Generic;

namespace lib
{
    public class GameState
    {
        public GameState(Map currentMap, int currentPunter, List<Move> previousMoves)
        {
            CurrentMap = currentMap;
            CurrentPunter = currentPunter;
            PreviousMoves = previousMoves;
        }

        public Map CurrentMap { get; }
        public int CurrentPunter { get; }
        public List<Move> PreviousMoves { get; }
    }
}