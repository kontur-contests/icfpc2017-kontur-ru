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
        public string Name => nameof(ConnectClosestMinesAi);
        public string Version => "0.1";

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            services.Setup<MineDistCalculator>(state);
            services.Setup<MeetingPointService>(state);

            var meetingPoint = state.mps.meetingPoint;

            var graph = services.Get<GraphService>(state).Graph;
            var futures = new List<Future>();
            foreach (var mine in graph.Mines.Keys)
            {
                futures.Add(new Future(mine, meetingPoint));
            }

            return AiSetupDecision.Create(futures.ToArray(), $"meet in {meetingPoint}");
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var meetingPoint = state.mps.meetingPoint;

            var graph = services.Get<GraphService>(state).Graph;
            var toDo = ConnectClosestMinesAi.GetNotMyMines(state, graph)
                .Select(x => x.Id);

            var myVerts = graph.Vertexes.Values
                .Where(v => 
                    v.Edges.Any(e => e.Owner == state.punter) || v.Id == meetingPoint)
                .Select(x => x.Id)
                .ToArray();

            var shortest = ShortestPathGraph.Build(graph, myVerts);

            foreach (var mine in toDo)
            {
                var len = shortest[mine].Distance;
                //if len < ...
                var path = shortest[mine].Edges;
                foreach (var edge in path)
                {
                    
                }
            }

            AiMoveDecision move;
            if (ConnectClosestMinesAi.TryExtendAnything(state, services, out move))
                return move;
            return AiMoveDecision.Pass(state.punter);
        }
    }
}