using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class BuildNewComponentStrategy : IStrategy
    {
        private readonly State state;
        private readonly Graph graph;

        public BuildNewComponentStrategy(State state, Graph graph)
        {
            this.state = state;
            this.graph = graph;
        }

        public AiSetupDecision Setup()
        {
            return null;
        }

        public List<TurnResult> NextTurns()
        {
            AiMoveDecision move;
            if (TryBuildNewComponent(out move))
                return new List<TurnResult> { new TurnResult { Move = move, Estimation = 1 } };
            return new List<TurnResult>();
        }

        private bool TryBuildNewComponent(out AiMoveDecision move)
        {
            var queue = new Queue<BuildQueueItem>();
            var used = new Dictionary<int, BuildQueueItem>();
            foreach (var mineV in graph.GetNotOwnedMines(state.punter))
            {
                var queueItem = new BuildQueueItem
                {
                    CurrentVertex = mineV,
                    SourceMine = mineV,
                    FirstEdge = null
                };
                queue.Enqueue(queueItem);
                used.Add(mineV.Id, queueItem);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var edge in current.CurrentVertex.Edges.Where(x => x.Owner == -1))
                {
                    var next = graph.Vertexes[edge.To];
                    BuildQueueItem prev;
                    if (used.TryGetValue(next.Id, out prev))
                    {
                        if (prev.SourceMine != current.SourceMine)
                        {
                            var bestMine = SelectBestMine(prev.SourceMine, current.SourceMine);
                            if (bestMine == prev.SourceMine)
                            {
                                var edge1 = prev.FirstEdge ?? edge;
                                move = AiMoveDecision.Claim(state.punter, edge1.From, edge1.To);
                                return true;
                            }
                            if (bestMine == current.SourceMine)
                            {
                                var edge1 = current.FirstEdge ?? edge;
                                move = AiMoveDecision.Claim(state.punter, edge1.From, edge1.To);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        var queueItem = new BuildQueueItem
                        {
                            CurrentVertex = next,
                            SourceMine = current.SourceMine,
                            FirstEdge = current.FirstEdge ?? edge
                        };
                        queue.Enqueue(queueItem);
                        used.Add(next.Id, queueItem);
                    }
                }
            }
            move = null;
            return false;
        }

        private static Vertex SelectBestMine(Vertex a, Vertex b)
        {
            return a.Edges.Count(x => x.Owner == -1) < b.Edges.Count(x => x.Owner == -1) ? a : b;
        }

        private class BuildQueueItem
        {
            public Vertex CurrentVertex;
            public Vertex SourceMine;
            public Edge FirstEdge;
        }
    }
}