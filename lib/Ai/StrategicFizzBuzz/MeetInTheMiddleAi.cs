using System;
using JetBrains.Annotations;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using lib.viz;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline]
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