using System.Collections.Generic;
using lib.GraphImpl;

namespace lib.Strategies
{
    public interface IStrategy
    {
        List<TurnResult> Turn(Graph graph);
    }
}