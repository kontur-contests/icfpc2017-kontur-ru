using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public class ConnectClosestMinesAi : IAi
    {
        private int punterId;
        private List<int> myMines = new List<int>();

        public string Name => nameof(ConnectClosestMinesAi);

        public void StartRound(int punterId, int puntersCount, Map map)
        {
            this.punterId = punterId;
            // precalc (site,mine) -> score
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            throw new InvalidOperationException();
            //if (myMines.Any())
            //{
            //    // bfs от myMines до ближайшей шахты
            //    // if (can take first River)
            //    //     return first River
            //}

            // bfs от всех, кроме myMines
            // if (has path)
            //     return first path item

            // foreach myMines.adjastentRivers take best

            // pass
        }

        public string SerializeGameState()
        {
            throw new System.NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new System.NotImplementedException();
        }
    }
}