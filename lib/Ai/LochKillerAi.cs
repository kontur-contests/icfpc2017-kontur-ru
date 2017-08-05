using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;

namespace lib.Ai
{
    public class LochKillerAi : IAi
    {
        public string Name => nameof(LochKillerAi);
        public string Version => "0.1";

        private GreedyAi Base = new GreedyAi();

        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            return Base.StartRound(punterId, puntersCount, map, settings);
        }

        Random rand = new Random();
        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            if (map.Sites.Length < 300)
                return Base.GetNextMove(prevMoves, map);

            var graph = new Graph(map);

            var playersCount = map.Rivers.Select(river => river.Owner).Distinct().Count(i => i >= 0);

            var nearMinesEdge = map.Mines
                .Select(mine => new { mine, edges = graph.Vertexes[mine].Edges.Select(edge => edge.River).ToList() })
                .Where(mine => mine.edges.Select(edge => edge.Owner).Distinct().Count() < playersCount+1)
                .OrderBy(mine => Tuple.Create(mine.edges.Select(edge => edge.Owner).Distinct().Count(), rand.Next()))
                .Where(mine => mine.edges.Count <= 100)
                .SelectMany(mine => mine.edges)
                .FirstOrDefault(edge => edge.Owner < 0);
            if (nearMinesEdge == null)
                return Base.GetNextMove(prevMoves, map);
            return new ClaimMove(Base.punterId, nearMinesEdge.Source, nearMinesEdge.Target);   
        }

        public string SerializeGameState()
        {
            return Base.SerializeGameState();
        }

        public void DeserializeGameState(string gameState)
        {
            Base.DeserializeGameState(gameState);
        }
    }

    public class LochMaxVertexWeighterKillerAi : IAi
    {
        public string Name => nameof(LochMaxVertexWeighterKillerAi);
        public string Version => "0.1";

        private IAi Base = new MaxReachableVertexWithConnectedComponentsWeightAi();

        private int punterId;
        private int puntersCount;

        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            this.puntersCount = puntersCount;
            return Base.StartRound(punterId, puntersCount, map, settings);
        }

        Random rand = new Random();
        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            if (map.Sites.Length / puntersCount < 150)
                return Base.GetNextMove(prevMoves, map);

            var graph = new Graph(map);

            var nearMinesEdge = map.Mines
                .Select(mine => new { mine, edges = graph.Vertexes[mine].Edges.Select(edge => edge.River).ToList() })
                .Where(mine => mine.edges.Select(edge => edge.Owner).Distinct().Count() < puntersCount + 1)
                .OrderBy(mine => Tuple.Create(mine.edges.Select(edge => edge.Owner).Distinct().Count(), rand.Next()))
                .Where(mine => mine.edges.Count <= 100)
                .SelectMany(mine => mine.edges)
                .FirstOrDefault(edge => edge.Owner < 0);
            if (nearMinesEdge == null)
                return Base.GetNextMove(prevMoves, map);
            return new ClaimMove(punterId, nearMinesEdge.Source, nearMinesEdge.Target);
        }

        public string SerializeGameState()
        {
            return Base.SerializeGameState();
        }

        public void DeserializeGameState(string gameState)
        {
            Base.DeserializeGameState(gameState);
        }
    }
}