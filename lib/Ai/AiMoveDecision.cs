using System;
using lib.GraphImpl;
using lib.Structures;

namespace lib.Ai
{
    public class AiMoveDecision
    {
        public readonly Move move;
        public readonly string reason;

        public AiMoveDecision(Move move, string reason = null)
        {
            this.move = move;
            this.reason = reason;
        }

        public static AiMoveDecision Claim(Edge edge, int punter, string reason = null)
        {
            if (edge == null)
                throw new InvalidOperationException("Attempt to claim null edge! WTF?");
            if (edge.IsFree)
                return new AiMoveDecision(Move.Claim(punter, edge.From, edge.To), reason);
            throw new InvalidOperationException($"Attempt to claim owned river {edge.River}! WTF?");
        }

        public static AiMoveDecision Option(Edge edge, int punter, string reason = null)
        {
            if (edge == null)
                throw new InvalidOperationException("Attempt to option null edge! WTF?");
            if (edge.IsFree)
                throw new InvalidOperationException("Attempt to option free edge! WTF?");
            if (!edge.CanBeOwnedBy(punter, true))
                throw new InvalidOperationException($"Attempt to option owned river {edge.River}! WTF?");
            return new AiMoveDecision(Move.Option(punter, edge.From, edge.To), reason);
        }

        public static AiMoveDecision Splurge(int punter, int[] siteIds, string reason = null)
        {
            return new AiMoveDecision(Move.Splurge(punter, siteIds), reason);
        }

        public static AiMoveDecision Pass(int punter, string reason = null)
        {
            return new AiMoveDecision(Move.Pass(punter), reason);
        }

        public static AiMoveDecision ClaimOrOption(Edge edge, int punter, bool haveFreeOption, string reason = null)
        {
            if (edge == null)
                throw new InvalidOperationException("Attempt to claim or option null edge! WTF?");
            if (edge.IsFree)
                return new AiMoveDecision(Move.Claim(punter, edge.From, edge.To), reason);
            if (edge.CanBeOwnedBy(punter, haveFreeOption))
                return new AiMoveDecision(Move.Option(punter, edge.From, edge.To), reason);
            throw new InvalidOperationException($"Attempt to claim or option owned river {edge.River}! WTF?");
        }

    }
}