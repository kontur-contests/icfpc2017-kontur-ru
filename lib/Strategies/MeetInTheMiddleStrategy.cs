using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Structures;

namespace lib.Strategies
{
    public class MeetInTheMiddleSetupStrategy : ISetupStrategy
    {
        private readonly Graph graph;
        private readonly MeetingPointService meetingPointService;

        public MeetInTheMiddleSetupStrategy(State state, IServices services)
        {
            graph = services.Get<Graph>();
            meetingPointService = services.Get<MeetingPointService>();
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
    }

    public class MeetInTheMiddleStrategy : IStrategy
    {
        private readonly State state;
        private readonly Graph graph;
        private readonly MeetingPointService meetingPointService;

        public MeetInTheMiddleStrategy(State state, IServices services)
        {
            this.state = state;
            graph = services.Get<Graph>();
            meetingPointService = services.Get<MeetingPointService>();
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

            var toDo = graph.GetNotOwnedMines(state.punter).Select(x => x.Id);

            var myVerts = graph.Vertexes.Values.Where(
                    v =>
                    v.Edges.Any(e => e.IsOwnedBy(state.punter)) || v.Id == meetingPoint)
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