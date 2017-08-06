using System.Collections.Generic;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public interface IStrategy
    {
        List<TurnResult> Turn(State state, IServices services);
    }
}