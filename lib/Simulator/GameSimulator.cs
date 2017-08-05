using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using MoreLinq;

namespace lib
{
    public class GameSimulator : ISimulator
    {
        private Map map;
        private List<IAi> punters;
        private int currentPunter = 0;
        private readonly List<Move> moves;
        private int turnsAmount;

        public GameSimulator(Map map)
        {
            this.map = map;
            punters = new List<IAi>();
            moves = new List<Move>();
        }

        public void StartGame(List<IAi> gamers)
        {
            punters = gamers;
            for (int i = 0; i < punters.Count; i++)
            {
                punters[i].StartRound(i, punters.Count, map);
            }

            turnsAmount = map.Rivers.Length;
        }

        public GameState NextMove()
        {
            if (turnsAmount <= 0)
                return new GameState(map, moves.TakeLast(punters.Count).ToList(), true);

            var nextMove = punters[currentPunter].GetNextMove(moves.ToArray(), map);
            ApplyMove(nextMove);
            moves.Add(nextMove);
            currentPunter = (currentPunter + 1) % punters.Count;
            turnsAmount--;
            return new GameState(map, moves.TakeLast(punters.Count).ToList(), false);
        }

        private void ApplyMove(Move nextMove)
        {
            map = nextMove.Execute(map);
        }
    }
}