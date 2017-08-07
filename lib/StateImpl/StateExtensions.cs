using System;
using System.Linq;
using lib.Ai;
using lib.Structures;

namespace lib.StateImpl
{
    public static class StateExtensions
    {
        public static void ApplyMoves(this State state, Move[] moves)
        {
            foreach (var move in moves.Skip(state.punter).Concat(moves.Take(state.punter)))
            {
                state.map = state.map.ApplyMove(move);
                if (move.pass != null)
                    state.credits[move.pass.punter] = state.credits.GetOrDefault(move.pass.punter, 0) + 1;
                else if (move.splurge != null)
                    state.credits[move.splurge.punter] = state.credits.GetOrDefault(move.splurge.punter, 0) - (move.splurge.SplurgeLength() - 1);
            }
            state.turns.Add(new TurnState
            {
                moves = moves,
                aiMoveDecision = state.lastAiMoveDecision
            });
        }

        public static void ValidateMove(this State state, AiInfoMoveDecision decision)
        {
            var move = decision.move;
            try
            {
                state.map.ApplyMove(decision);
            }
            catch (Exception e)
            {
                throw new InvalidDecisionException("invalid move", $"BUG in Ai - {decision}", e);
            }
            if (move.splurge != null)
            {
                if (state.credits[state.punter] < move.splurge.SplurgeLength())
                    throw new InvalidDecisionException("no credits", $"BUG in Ai - {decision} - Not enough credits (have {state.credits[state.punter]}, required {move.splurge.SplurgeLength()})");
            }
        }
    }
}