using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class DinicWeighter : IEdgeWeighter
    {
        private Random rand = new Random();
        private Graph Graph { get; }
        private int PunterId { get; }

        Dictionary<Tuple<int, int>, double> EdgesToBlock = new Dictionary<Tuple<int, int>, double>();

        private Tuple<int, int> ConvertToTuple(Edge edge)
        {
            return edge.From > edge.To ? Tuple.Create(edge.To, edge.From) : Tuple.Create(edge.From, edge.To);
        }

        public DinicWeighter(State state, IServices services)
        {
            Graph = services.Get<Graph>();
            PunterId = state.punter;
        }
        
        public void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            int maxCount = 10;
            EdgesToBlock.Clear();
            
            var mineToSave = Graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => edge.Owner != PunterId))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.Owner < 0) < PunterId).Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => rand.Next()).FirstOrDefault(edge => edge.Owner < 0);
                if (edgeToSave != null)
                    EdgesToBlock[ConvertToTuple(edgeToSave)] = 10;
            }

            var bannedMines = Graph.Mines
                .Where(mine => mine.Value.Edges.Select(edge => edge.Owner).Distinct().Count() == PunterId + 1)
                .Select(mine => mine.Key)
                .ToHashSet();
            
            var mines = Graph.Mines.Where(mine => mine.Value.Edges.Any(edge => edge.Owner < 0)).ToList();
            for (int i = 0; i < Math.Min(10, mines.Count * (mines.Count - 1)); i++)
            {
                var mine1 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];
                var mine2 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];
                while (mine2.Key == mine1.Key) mine2 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];

                var dinic = new Dinic(Graph, PunterId, mine1.Key, mine2.Key, out var flow);
                if (flow == 0)
                    continue;
                if (flow > maxCount)
                    continue;

                foreach (var edge in dinic.GetMinCut().Select(ConvertToTuple))
                {
                    if (bannedMines.Contains(edge.Item1) || bannedMines.Contains(edge.Item2))
                        continue;
                    EdgesToBlock[edge] = EdgesToBlock.GetOrDefault(edge, 0) + 1.0 / flow;
                }
            }
        }

        public double EstimateWeight(Edge edge)
        {
            return EdgesToBlock.GetOrDefault(ConvertToTuple(edge), 0);
        }
    }
}
