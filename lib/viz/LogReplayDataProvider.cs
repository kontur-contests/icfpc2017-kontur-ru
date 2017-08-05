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

        public LogReplayDataProvider(ReplayFullData data)
        {
            this.data = data;
            map = data.Data.Map;
            PunterNames = data.Meta.Scores
                .Select((s, i) => i == data.Meta.OurPunter ? data.Meta.AiName : i.ToString())
                .ToArray();
        }

        public string[] PunterNames { get; }
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