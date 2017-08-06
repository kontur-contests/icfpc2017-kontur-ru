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

        public DinicWeighter(State state, IServices services)
        {
            Graph = services.Get<Graph>();
            PunterId = state.punter;
        }
        
        public void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            int maxCount = 10;
            Dictionary<Tuple<int, int>, double> edgesToBlock = new Dictionary<Tuple<int, int>, double>();

            var mineToSave = Graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => edge.Owner != PunterId))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.Owner < 0) < PunterId).Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => rand.Next()).FirstOrDefault(edge => edge.Owner < 0);
                //if (edgeToSave != null)
                //    return AiMoveDecision.Claim(state.punter, edgeToSave.From, edgeToSave.To);
            }

            var bannedMines = Graph.Mines
                .Where(mine => mine.Value.Edges.Select(edge => edge.Owner).Distinct().Count() == PunterId + 1)
                .Select(mine => mine.Key)
                .ToHashSet();
        }

        public double EstimateWeight(Edge edge)
        {
            throw new NotImplementedException();
        }
    }
}
