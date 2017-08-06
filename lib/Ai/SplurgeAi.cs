using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Structures;
using lib.viz;

namespace lib.Ai
{
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class SplurgeExample : IAi
    {
        public string Name => "SplurgeExample";
        public string Version => "0";

        public AiSetupDecision Setup(State state, IServices services)
        {
            return AiSetupDecision.Create(new Future[0]);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            if (step++ == 4)
            {
                return AiMoveDecision.Splurge(state.punter, new []{0, 398, 364, 210, 480, 398});
            }
            return AiMoveDecision.Pass(state.punter);
        }

        public int step = 0;
    }
}