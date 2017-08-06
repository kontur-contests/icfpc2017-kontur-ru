using JetBrains.Annotations;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.Strategies.StrategiesCatalog;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class FutureIsNowUberAi : UberfullessnessAi
    {
        public FutureIsNowUberAi()
            : base(
                StrategyName.ForSetup<FutureIsNowSetupStrategy>(),
                StrategyName.For<FutureIsNowStrategy>(),
                StrategyName.For<NopStrategy>(),
                StrategyName.ForEWStrategy<AllComponentsEWStrategy, MaxVertextWeighter>())
        {
        }
    }
}