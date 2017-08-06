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
        private GraphService GraphService { get; }
        private int PunterId { get;  }

        public DinicWeighter(State state, IServices services)
        {
            GraphService = services.Get<GraphService>(state);
            PunterId = state.punter;
        }
        
        public void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            var graph = GraphService.Graph;
            int maxCount = 10;
            Dictionary<Tuple<int, int>, double> edgesToBlock = new Dictionary<Tuple<int, int>, double>();

            var mineToSave = graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => edge.Owner != PunterId))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.Owner < 0) < PunterId).Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => rand.Next()).FirstOrDefault(edge => edge.Owner < 0);
                //if (edgeToSave != null)
                //    return AiMoveDecision.Claim(state.punter, edgeToSave.From, edgeToSave.To);
            }

            var bannedMines = graph.Mines
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
