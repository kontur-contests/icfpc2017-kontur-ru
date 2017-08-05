using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;

namespace lib.Ai
{
    public class LochKillerAi : IAi
    {
        public string Name => nameof(LochKillerAi);

        private GreedyAi Base = new GreedyAi();

        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            return Base.StartRound(punterId, puntersCount, map, settings);
        }

        Random rand = new Random();
        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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