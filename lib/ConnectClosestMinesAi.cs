using System;
using System.Collections.Generic;

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

        public IMove GetNextMove(IMove[] prevMoves, Map map)
        {
            throw new NotImplementedException();
            //if (myMines.Any())
            //{
            //    // bfs �� myMines �� ��������� �����
            //    // if (can take first River)
            //    //     return first River
            //}

            // bfs �� ����, ����� myMines
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