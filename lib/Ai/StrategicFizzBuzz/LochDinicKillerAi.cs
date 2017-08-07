using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.Strategies.lib.Strategies;
using lib.viz;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochDinicKillerAi : CompositeStrategicAi
    {
        public LochDinicKillerAi()
            : base(
                (state, services) => new LochDinicKillerStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.4";
    }

    public class LochDinicKillerAi2 : CompositeStrategicAi
    {
        public LochDinicKillerAi2()
            : base(
                (state, services) => new LochDinicKillerStrategy2(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.4";
    }

    public class OptAntiLochDinicKillerAi : CompositeStrategicAi
    {
        public OptAntiLochDinicKillerAi()
            : base(
                (state, services) => new AntiLochDinicStrategy(true, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), -1),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.4";
    }

    public class AntiLochDinicKillerAi : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.4";
    }

    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_0 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_0()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_04 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_04()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.4),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_1 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_1()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 1),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_05 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_05()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.5),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_03 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_03()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.3),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_02 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_02()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.2),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class AntiLochDinicKillerAi_01 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_01()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.1),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    public class AntiLochDinicKillerAi_005 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_005()
            : base(
                (state, services) => new AntiLochDinicStrategy(false, state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.05),
                (state, services) => new GreedyStrategy(true, state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
}