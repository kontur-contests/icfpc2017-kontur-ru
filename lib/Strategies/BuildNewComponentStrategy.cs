using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class BuildNewComponentStrategy : IStrategy
    {
        private readonly bool allowToUseOptions;
        private readonly State state;
        private readonly Graph graph;

        public BuildNewComponentStrategy(bool allowToUseOptions, State state, IServices services)
        {
            this.allowToUseOptions = state.settings.options && allowToUseOptions && state.map.OptionsLeft(state.punter) > 0;
            this.state = state;
            graph = services.Get<Graph>();
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
                foreach (var edge in current.CurrentVertex.Edges.Where(x => x.CanBeOwnedBy(state.punter, allowToUseOptions)))
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
                                move = AiMoveDecision.ClaimOrOption(edge1, state.punter, allowToUseOptions);
                                return true;
                            }
                            if (bestMine == current.SourceMine)
                            {
                                var edge1 = current.FirstEdge ?? edge;
                                move = AiMoveDecision.ClaimOrOption(edge1, state.punter, allowToUseOptions);
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

        private Vertex SelectBestMine(Vertex a, Vertex b)
        {
            return a.Edges.Count(x => x.CanBeOwnedBy(state.punter, allowToUseOptions)) < b.Edges.Count(x => x.CanBeOwnedBy(state.punter, allowToUseOptions)) ? a : b;
        }

        private class BuildQueueItem
        {
            public Vertex CurrentVertex;
            public Vertex SourceMine;
            public Edge FirstEdge;
        }
    }
}