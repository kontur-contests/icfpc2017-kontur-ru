using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.Structures;
using MoreLinq;

namespace lib
{
    public class GameSimulator : ISimulator
    {
        private Map map;
        private readonly Settings settings;
        private List<IAi> punters;
        private int currentPunter = 0;
        private readonly List<Move> moves;
        private int turnsAmount;

        public IList<Future[]> Futures { get; }

        public GameSimulator(Map map, Settings settings)
        {
            this.map = map;
            this.settings = settings;
            punters = new List<IAi>();
            moves = new List<Move>();
            Futures = new List<Future[]>();
        }

        public void StartGame(List<IAi> gamers)
        {
            punters = gamers;
            for (int i = 0; i < punters.Count; i++)
            {
                var punterFutures = punters[i].StartRound(i, punters.Count, map, settings);

                Futures.Add(ValidateFutures(punterFutures));
            }

            turnsAmount = map.Rivers.Length;
        }

        public GameState NextMove()
        {
            if (turnsAmount <= 0)
                return new GameState(map, moves.TakeLast(punters.Count).ToList(), true);

            var nextMove = punters[currentPunter].GetNextMove(moves.ToArray(), map);
            map = map.ApplyMove(nextMove);
            moves.Add(nextMove);
            currentPunter = (currentPunter + 1) % punters.Count;
            turnsAmount--;
            return new GameState(map, moves.TakeLast(punters.Count).ToList(), false);
        }

        private Future[] ValidateFutures(Future[] futures)
            => futures
                .Where(e => map.Mines.Contains(e.source) && !map.Mines.Contains(e.target))
                .GroupBy(e => e.source)
                .Select(e => e.Last())
                .ToArray();
    }
}