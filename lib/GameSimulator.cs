using System.Collections.Generic;

namespace lib
{
    public class GameSimulator : ISimulator
    {
        private readonly Map map;
        private List<IAi> punters;
        private int currentPunter = 0;
        private readonly List<Move> moves;

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
        }

        public GameState NextMove()
        {
            var nextMove = punters[currentPunter].GetNextMove(moves.ToArray(), map);

            ApplyMove(nextMove);

            moves.Add(nextMove);
            currentPunter++;
            return new GameState(map, currentPunter, moves);
        }

        private void ApplyMove(Move nextMove)
        {
            foreach (var river in map.Rivers)
                if (river.Source == nextMove.Source && river.Target == nextMove.Target)
                {
                    river.Owner = nextMove.PunterId;
                    return;
                }
        }
    }
}