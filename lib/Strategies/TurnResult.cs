using lib.Ai;
using lib.Structures;

namespace lib.Strategies
{
    public class TurnResult
    {
        public double Estimation { get; set; }
        public AiMoveDecision Move { get; set; }
    }
}