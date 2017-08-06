using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using lib.viz;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline] // T-16:30
    public class MaxReachableVertexWeightAiWithMineGuardianAi : CompositeStrategicAi
    {
        public MaxReachableVertexWeightAiWithMineGuardianAi()
            : this(3, 10, 100)
        {
        }

        public MaxReachableVertexWeightAiWithMineGuardianAi(int outEdgesWarningLevel, int guardedComponentSizeThreshold, int mineMultiplier)
            : base(
                AllComponentsEWStrategy((state, services) => new MineGuardianEdgeWeighter(state, services, outEdgesWarningLevel, guardedComponentSizeThreshold)),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(mineMultiplier, state, services)))
        {
        }

        public override string Version => "1.1";
    }
}