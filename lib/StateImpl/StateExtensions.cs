using lib.Structures;

namespace lib.StateImpl
{
    public static class StateExtensions
    {
        public static void ApplyMoves(this State state, Move[] moves)
        {
            foreach (var move in moves)
            {
                state.map = state.map.ApplyMove(move);
                if (move.pass != null)
                    state.credits[move.pass.punter] = state.credits.GetOrDefault(move.pass.punter, 0) + 1;
                else if (move.splurger != null)
                    state.credits[move.splurger.punter] -= move.splurger.SplurgeLength() - 1;
            }
            state.turns.Add(new TurnState
            {
                moves = moves,
                aiMoveDecision = state.lastAiMoveDecision
            });
        }
    }
}