using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace lib
{
    public class GameSimulator : ISimulator
    {
        private readonly Map map;
        private List<IAi> punters;
        private int currentPunter = 0;
        private readonly List<IMove> moves;

        public GameSimulator(Map map)
        {
            this.map = map;
            punters = new List<IAi>();
            moves = new List<IMove>();
        }

        public void StartGame(List<IAi> gamers)
        {
            punters = gamers;
            for (int i = 0; i < punters.Count; i++)
            {
                punters[i].StartRound(i, punters.Count, map);
            }
        }

        public GameState NextMove()
        {
            var nextMove = punters[currentPunter].GetNextMove(moves.ToArray(), map);

            ApplyMove(nextMove);

            moves.Add(nextMove);
            currentPunter = (currentPunter + 1) % punters.Count;
            return new GameState(map, currentPunter, moves.TakeLast(punters.Count).ToList());
        }

        private void ApplyMove(IMove nextMove)
        {
            if (nextMove is Move move)
            {
                foreach (var river in map.Rivers)
                    if (river.Source == move.Source && river.Target == move.Target)
                    {
                        river.Owner = move.PunterId;
                        return;
                    }
            }
        }
    }
}