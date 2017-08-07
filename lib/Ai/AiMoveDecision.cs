using lib.Structures;

namespace lib.Ai
{
    public class AiMoveDecision
    {
        public readonly Move move;
        public readonly string reason;

        private AiMoveDecision(Move move, string reason = null)
        {
            this.move = move;
            this.reason = reason;
        }

        public static AiMoveDecision Move(Move move, string reason = null)
        {
            return new AiMoveDecision(move, reason);
        }

        public static AiMoveDecision Claim(int punter, int source, int target, string reason = null)
        {
            return Move(Structures.Move.Claim(punter, source, target), reason);
        }

        public static AiMoveDecision Option(int punter, int source, int target, string reason = null)
        {
            return Move(Structures.Move.Option(punter, source, target), reason);
        }

        public static AiMoveDecision Splurge(int punter, int[] siteIds, string reason = null)
        {
            return Move(Structures.Move.Splurge(punter, siteIds), reason);
        }

        public static AiMoveDecision Pass(int punter, string reason = null)
        {
            return Move(Structures.Move.Pass(punter), reason);
        }
    }
}