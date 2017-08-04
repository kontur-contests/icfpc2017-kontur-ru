using System.Collections.Generic;

namespace lib
{
    public class GameState
    {
        public GameState(Map currentMap, int currentPunter, List<IMove> previousMoves, bool isGameOver)
        {
            CurrentMap = currentMap;
            CurrentPunter = currentPunter;
            PreviousMoves = previousMoves;
            IsGameOver = isGameOver;
        }

        public Map CurrentMap { get; }
        public int CurrentPunter { get; }
        public List<IMove> PreviousMoves { get; }
        public bool IsGameOver { get; }
    }
} 