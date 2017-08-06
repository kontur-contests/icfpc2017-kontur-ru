using System.Collections.Generic;

namespace lib.Strategies
{
    public class NopStrategy : IStrategy
    {
        public List<TurnResult> NextTurns()
        {
            return new List<TurnResult>();
        }
    }
}