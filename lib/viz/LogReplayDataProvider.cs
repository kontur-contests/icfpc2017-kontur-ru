using System.Collections.Generic;
using System.Linq;
using lib.Structures;

namespace lib.viz
{
    internal class LogReplayDataProvider : IReplayDataProvider
    {
        private readonly ReplayFullData data;
        private readonly List<Move> prevMoves = new List<Move>();
        private int nextMoveIndex;
        private Map map;
        private readonly int ourPunterIndex;
        private readonly Future[] futures;
        private readonly long[] splurges;

        public LogReplayDataProvider(ReplayFullData data)
        {
            this.data = data;
            map = data.Data.Map;
            PunterNames = data.Meta.Scores
                .Select((s, i) => i == data.Meta.OurPunter ? data.Meta.AiName : i.ToString())
                .ToArray();
            ourPunterIndex = data.Meta.OurPunter;
            splurges = new long[PunterNames.Length];
            futures = data.Data.Futures ?? new Future[0];
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
                var move = data.Data.Moves[nextMoveIndex++];
                map = map.ApplyMove(move);
                prevMoves.Add(move);
                splurges.Update(move);
            }
            return new GameState(map, prevMoves, nextMoveIndex >= data.Data.Moves.Length, splurges);
        }
    }
}