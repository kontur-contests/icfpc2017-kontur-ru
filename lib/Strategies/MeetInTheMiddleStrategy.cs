using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Structures;

namespace lib.Strategies
{
    public class MeetInTheMiddleStrategy : IStrategy
    {
        private readonly State state;
        private readonly Graph graph;
        private readonly MeetingPointService meetingPointService;

        public MeetInTheMiddleStrategy(State state, Graph graph, MeetingPointService meetingPointService)
        {
            this.state = state;
            this.graph = graph;
            this.meetingPointService = meetingPointService;
        }

        public AiSetupDecision Setup()
        {
            var meetingPoint = meetingPointService.MeetingPoint;

            var futures = new List<Future>();
            foreach (var mine in graph.Mines.Keys)
            {
                futures.Add(new Future(mine, meetingPoint));
            }
            futures = futures.Where(f => f.source != f.target).ToList();

            return AiSetupDecision.Create(futures.ToArray(), $"meet in {meetingPoint}");
        }

        public List<TurnResult> NextTurns()
        {
            var decision = TryGetNextMove();
            if (decision != null)
                return new List<TurnResult> { new TurnResult { Move = decision, Estimation = 1 } };
            return new List<TurnResult>();
        }

        private AiMoveDecision TryGetNextMove()
        {
            var meetingPoint = meetingPointService.MeetingPoint;

            var toDo = Enumerable.Select<Vertex, int>(graph.GetNotOwnedMines(state.punter), x => x.Id);

            var myVerts = Enumerable.Where<Vertex>(
                    graph.Vertexes.Values, v =>
                    Enumerable.Any<Edge>(v.Edges, e => e.Owner == state.punter) || v.Id == meetingPoint)
                .Select(x => x.Id)
                .ToList();

            var shortest = new ShortestPathFinder(graph, state.punter, myVerts);

            var skip = false;

            foreach (var mine in toDo)
            {
                var path = shortest.GetPath(mine);
                if (path == null)
                    continue;

                int len = path.Count - 1;
                if (len > state.credits[state.punter])
                {
                    skip = true;
                    continue;
                }

                return AiMoveDecision.Splurge(state.punter, path.ToArray());
            }

            if (skip)
                return AiMoveDecision.Pass(state.punter, "wait");
            return null;
        }

    }
}