using System.Collections.Generic;
using lib.StateImpl;
using lib.Structures;

namespace lib
{
    public class GameState
    {
        public GameState(Map currentMap, List<Move> previousMoves, bool isGameOver, long[] splurgePoints)
        {
            CurrentMap = currentMap;
            PreviousMoves = previousMoves;
            IsGameOver = isGameOver;
            SplurgePoints = splurgePoints;
        }

        public long[] SplurgePoints { get; }
        public Map CurrentMap { get; }
        public List<Move> PreviousMoves { get; }
        public bool IsGameOver { get; }
    }
} 