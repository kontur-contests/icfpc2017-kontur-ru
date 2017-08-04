using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;

namespace lib.Strategies
{
    public class GreedyAi : IAi
    {
        public string Name => nameof(GreedyAi);
        private int punterId;

        private MineDistCalculator mineDistCalulator;

        // ReSharper disable once ParameterHidesMember
        public void StartRound(int punterId, int puntersCount, Map map)
        {
            this.punterId = punterId;
            this.mineDistCalulator = new MineDistCalculator(new Graph(map));
        }

        public IMove GetNextMove(IMove[] prevMoves, Map map)
        {
            var graph = new Graph(map);
            var calculator = new ConnectedCalculator(graph, punterId);
            


            return new Pass();
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