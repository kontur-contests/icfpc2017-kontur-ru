using System.Collections.Generic;
using lib.Structures;

namespace lib
{
    public class GameState
    {
        public GameState(Map currentMap, List<Move> previousMoves, bool isGameOver)
        {
            CurrentMap = currentMap;
            PreviousMoves = previousMoves;
            IsGameOver = isGameOver;
        }

        public Map CurrentMap { get; }
        public List<Move> PreviousMoves { get; }
        public bool IsGameOver { get; }
    }
} 