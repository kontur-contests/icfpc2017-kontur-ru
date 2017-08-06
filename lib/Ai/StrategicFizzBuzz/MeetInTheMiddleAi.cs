using System;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;

namespace lib.Ai.StrategicFizzBuzz
{
    public class MeetInTheMiddleAi : CompositeStrategicAi
    {
        public MeetInTheMiddleAi()
            : base(
                (state, services) => new MeetInTheMiddleStrategy(state, services.Get<Graph>(), services.Get<MeetingPointService>()),
                (state, services) => new GreedyStrategy(state, services, Math.Max))
        {
        }

        public override string Version => "0.1";
    }
}