using System.Collections.Generic;
using lib.GraphImpl;
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
            services.Setup<Graph>();
            services.Setup<MineDistCalculator>();
            services.Setup<MeetingPointService>();

            var meetingPoint = state.mps.meetingPoint;

            var graph = services.Get<Graph>();
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

            AiMoveDecision move;
            if (ConnectClosestMinesAi.TryExtendComponent(state, services, out move))
                return move;
            if (ConnectClosestMinesAi.TryBuildNewComponent(state, services, out move))
                return move;
            if (ConnectClosestMinesAi.TryExtendAnything(state, services, out move))
                return move;
            return AiMoveDecision.Pass(state.punter);
        }
    }
}