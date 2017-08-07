using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class ExtendComponentStrategy : IStrategy
    {
        private readonly bool allowToUseOptions;
        private readonly State state;
        private readonly Graph graph;

        public ExtendComponentStrategy(bool allowToUseOptions, State state, IServices services)
        {
            this.allowToUseOptions = state.settings.options && allowToUseOptions && state.map.OptionsLeft(state.punter) > 0;
            this.state = state;
            graph = services.Get<Graph>();
        }

        public List<TurnResult> NextTurns()
        {
            AiMoveDecision move;
            if (TryExtendComponent(out move))
                return new List<TurnResult> { new TurnResult { Move = move, Estimation = 1 } };
            return new List<TurnResult>();
        }

        private bool TryExtendComponent(out AiMoveDecision move)
        {
            //TODO Сейчас увеличивает первую попавшуюся компоненту. А может быть нужно расширять самую большую компоненту.
            var queue = new Queue<ExtendQueueItem>();
            var used = new HashSet<int>();
            foreach (var mineV in graph.GetNotOwnedMines(state.punter))
            {
                var queueItem = new ExtendQueueItem
                {
                    CurrentVertex = mineV,
                    Edge = null
                };
                queue.Enqueue(queueItem);
                used.Add(mineV.Id);
            }
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.CurrentVertex.Edges.Any(x => x.IsOwnedBy(state.punter)))
                {
                    move = AiMoveDecision.ClaimOrOption(current.Edge, state.punter, allowToUseOptions);
                    return true;
                }
                foreach (var edge in current.CurrentVertex.Edges.Where(x => x.CanBeOwnedBy(state.punter, allowToUseOptions)))
                {
                    var next = graph.Vertexes[edge.To];
                    if (!used.Contains(next.Id))
                    {
                        var queueItem = new ExtendQueueItem
                        {
                            CurrentVertex = next,
                            Edge = edge
                        };
                        queue.Enqueue(queueItem);
                        used.Add(next.Id);
                    }
                }
            }
            move = null;
            return false;
        }

        private class ExtendQueueItem
        {
            public Vertex CurrentVertex;
            public Edge Edge;
        }
    }
}