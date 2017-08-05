using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;

namespace lib.Ai
{
    class LochKillerAi : ConnectClosestMinesAi
    {
        public new string Name => nameof(LochKillerAi);

        private readonly int PunterId;
        public Graph Graph;

        Random rand = new Random();

        public LochKillerAi(Graph graph, int panterId)
        {
            Graph = graph;
            PunterId = panterId;
        }

        public new Move GetNextMove(Move[] prevMoves, Map map)
        {
            var nearMinesEdge = map.Mines
                .Select(mine => new{mine, edges = Graph.Vertexes[mine].Edges.Select(edge => edge.River).ToList()})
                .OrderBy(mine => Tuple.Create(mine.edges.Select(edge => edge.Owner).Distinct().Count(), rand.Next()))
                .SelectMany(mine => mine.edges)
                .FirstOrDefault(edge => edge.Owner < 0);
            if (nearMinesEdge == null)
                return base.GetNextMove(prevMoves, map);
            return new ClaimMove(PunterId, nearMinesEdge.Source, nearMinesEdge.Target);
        }
    }
}
