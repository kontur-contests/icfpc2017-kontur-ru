using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;

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
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
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