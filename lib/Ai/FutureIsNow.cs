using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.Structures;
using lib.viz;
using lib.viz.Detalization;
using NUnit.Framework;

namespace lib.Ai
{
    public class FutureIsNow : IAi
    {
        public string Name => "Futurer";
        public string Version => "0";
        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            throw new System.NotImplementedException(); 
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            throw new System.NotImplementedException();
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