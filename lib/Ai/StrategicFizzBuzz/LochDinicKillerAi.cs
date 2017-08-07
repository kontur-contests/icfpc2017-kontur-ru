using lib.Ai.StrategicFizzBuzz.lib.Strategies;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class LochDinicKillerAi : CompositeStrategicAi
    {
        public LochDinicKillerAi()
            : base(
                (state, services) => new LochDinicKillerStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)),
                (state, services) => new GreedyStrategy(state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.4";
    }

    public class AntiLochDinicKillerAi : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi()
            : base(
                (state, services) => new AntiLochDinicStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services)),
                (state, services) => new GreedyStrategy(state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.4";
    }

    public class AntiLochDinicKillerAi_0 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_0()
            : base(
                (state, services) => new AntiLochDinicStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0),
                (state, services) => new GreedyStrategy(state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    public class AntiLochDinicKillerAi_09 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_09()
            : base(
                (state, services) => new AntiLochDinicStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.9),
                (state, services) => new GreedyStrategy(state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    public class AntiLochDinicKillerAi_1 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_1()
            : base(
                (state, services) => new AntiLochDinicStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 1),
                (state, services) => new GreedyStrategy(state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
    public class AntiLochDinicKillerAi_05 : CompositeStrategicAi
    {
        public AntiLochDinicKillerAi_05()
            : base(
                (state, services) => new AntiLochDinicStrategy(state, services),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(100, state, services), 0.5),
                (state, services) => new GreedyStrategy(state, services, (x, y) => x + y))
        {
        }

        public override string Version => "0.1";
    }
}