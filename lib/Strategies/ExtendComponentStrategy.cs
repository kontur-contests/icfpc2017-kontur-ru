using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class ExtendComponentStrategy : IStrategy
    {
        private readonly State state;
        private readonly Graph graph;

        public ExtendComponentStrategy(State state, Graph graph)
        {
            this.state = state;
            this.graph = graph;
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
                if (current.CurrentVertex.Edges.Any(x => x.Owner == state.punter))
                {
                    if (current.Edge == null)
                        throw new InvalidOperationException("Mine is already part of component! WTF?");
                    move = AiMoveDecision.Claim(state.punter, current.Edge.From, current.Edge.To);
                    return true;
                }
                foreach (var edge in current.CurrentVertex.Edges.Where(x => x.Owner == -1))
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