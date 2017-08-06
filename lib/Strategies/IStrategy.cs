using System.Collections.Generic;
using lib.Ai;

namespace lib.Strategies
{
    public interface IStrategy
    {
        AiSetupDecision Setup();
        List<TurnResult> NextTurns();
    }
}