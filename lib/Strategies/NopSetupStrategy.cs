using lib.Ai;

namespace lib.Strategies
{
    public class NopSetupStrategy : ISetupStrategy
    {
        public AiSetupDecision Setup()
        {
            return AiSetupDecision.Empty();
        }
    }
}