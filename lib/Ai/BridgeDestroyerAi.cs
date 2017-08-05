using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;
using lib.Strategies;
using lib.Structures;

namespace lib.Ai
{
    class BridgeDestroyerAi : IAi
    {
        public string Name => "BridgeDestroyer";
        public string Version => "0.1";

        private List<Tuple<int, int, double>> bridgesList;
        private int idx = 0;
        private int punter;

        Random rand = new Random();

        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            punter = punterId;
            return new Future[0];
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var graph = new Graph(map);
            MonteCarloBridgeSearcher searcher = new MonteCarloBridgeSearcher(graph, new MineDistCalculator(graph));
            searcher.BuildBridges();
            bridgesList = searcher.Bridges
                .OrderByDescending(pair => pair.Value)
                .ThenBy(_ => rand.Next())
                .Select(pair => Tuple.Create(pair.Key.From, pair.Key.To, pair.Value))
                .ToList();

            for (; idx < bridgesList.Count; idx++)
            {
                if(graph.Vertexes[bridgesList[idx].Item1].Edges.Where(edge => edge.To == bridgesList[idx].Item2).All(edge => edge.Owner >= 0))
                    continue;

                return Move.Claim(punter, bridgesList[idx].Item1, bridgesList[idx].Item2);
            }
            return Move.Pass(punter);

        }

        public string SerializeGameState()
        {
            throw new NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new NotImplementedException();
        }
    }
}
