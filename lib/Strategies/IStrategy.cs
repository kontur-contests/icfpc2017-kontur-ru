using System.Collections.Generic;

namespace lib.Strategies
{
    public interface IStrategy
    {
        void Init(Map map);
        List<TurnResult> Turn(Map map);
    }
}