using System;
using JetBrains.Annotations;
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
                (state, services) => new MeetInTheMiddleSetupStrategy(state, services),
                (state, services) => new MeetInTheMiddleStrategy(state, services),
                (state, services) => new GreedyStrategy(true, state, services, Math.Max))
        {
        }

        public override string Version => "0.1";
    }
}