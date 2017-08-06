using System.Collections.Generic;

namespace lib.Strategies
{
    public interface IStrategy
    {
        List<TurnResult> NextTurns();
    }
}