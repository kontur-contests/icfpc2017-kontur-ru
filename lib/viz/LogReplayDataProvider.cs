using System.Collections.Generic;
using System.Linq;

namespace lib.viz
{
    internal class LogReplayDataProvider : IReplayDataProvider
    {
        private readonly ReplayFullData data;
        private readonly List<Move> prevMoves = new List<Move>();
        private int nextMoveIndex;
        private Map map;
        private int ourPunterIndex;
        private Future[] futures;

        public LogReplayDataProvider(ReplayFullData data)
        {
            this.data = data;
            map = data.Data.Map;
            PunterNames = data.Meta.Scores
                .Select((s, i) => i == data.Meta.OurPunter ? data.Meta.AiName : i.ToString())
                .ToArray();
            futures = data.Data.Futures;
        }

        public string[] PunterNames { get; }

        public Future[] GetPunterFutures(int index)
        {
            return index == ourPunterIndex ? futures : new Future[0];
        }

        public GameState NextMove()
        {
            if (nextMoveIndex < data.Data.Moves.Length)
            {
                var move = data.Data.Moves[nextMoveIndex++].ToMove();
                map = move.Execute(map);
                prevMoves.Add(move);
            }
            return new GameState(map, prevMoves, nextMoveIndex >= data.Data.Moves.Length);
        }
    }
}