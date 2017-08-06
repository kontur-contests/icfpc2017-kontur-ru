using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.StateImpl;
using lib.Structures;

namespace lib.Ai
{
    public class MeetInTheMiddleAi : IAi
    {
        public string Name => GetType().Name;
        public string Version => "0.1";

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<Graph>();
            services.Setup<MineDistCalculator>();
            services.Setup<MeetingPointService>();

            var meetingPoint = services.Get<MeetingPointService>().MeetingPoint;

            var graph = services.Get<Graph>();
            var futures = new List<Future>();
            foreach (var mine in graph.Mines.Keys)
            {
                futures.Add(new Future(mine, meetingPoint));
            }
            futures = futures.Where(f => f.source != f.target).ToList();

            return AiSetupDecision.Create(futures.ToArray(), $"meet in {meetingPoint}");
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var meetingPoint = services.Get<MeetingPointService>().MeetingPoint;

            var graph = services.Get<Graph>();
            var toDo = ConnectClosestMinesAi.GetNotMyMines(state, graph)
                .Select(x => x.Id);

            var myVerts = graph.Vertexes.Values
                .Where(v => 
                    v.Edges.Any(e => e.Owner == state.punter) || v.Id == meetingPoint)
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

            AiMoveDecision move;
            if (ConnectClosestMinesAi.TryExtendAnything(state, services, out move))
                return move;
            return AiMoveDecision.Pass(state.punter);
        }
    }
}