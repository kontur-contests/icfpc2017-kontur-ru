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
    [ShoulNotRunOnline]
    public class FutureIsNow : IAi
    {
        private List<int> path;
        private int punterId;
        public string Name => "Futurer";
        public string Version => "0";
        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            var mineDists = new MineDistCalculator(new Graph(map));
            path = new PathSelector(map, mineDists).SelectPath(10);
            var futures = new FuturesPositioner(map, path, mineDists).GetFutures();
            return futures;
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            if (path.Count <= 1)
            {
                return new GameplayOut { pass = new PassMove() };
            }
            var claimMove = new ClaimMove { punter = punterId, source = path[0], target = path[1] };
            path.RemoveAt(0);
            return new GameplayOut { claim = claimMove };
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


    public class MovesSelector
    {
        private readonly Map map;
        private readonly int[] sitesToDefend;
        private readonly int punterId;

        public MovesSelector(Map map, int[] sitesToDefend, int punterId)
        {
            this.map = map;
            this.sitesToDefend = sitesToDefend;
            this.punterId = punterId;
        }

    }
}