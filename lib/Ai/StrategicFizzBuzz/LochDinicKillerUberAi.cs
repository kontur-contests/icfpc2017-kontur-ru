using JetBrains.Annotations;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.Strategies.StrategiesCatalog;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class LochDinicKillerUberAi : UberfullessnessAi
    {
        public LochDinicKillerUberAi()
            : base(
                StrategyName.ForSetup<NopSetupStrategy>(),
                StrategyName.For<LochDinicKillerStrategy>(),
                StrategyName.For<NopStrategy>(),
                StrategyName.ForEWStrategy<AllComponentsEWStrategy, MaxVertextWeighter>())
        {
        }
    }
}