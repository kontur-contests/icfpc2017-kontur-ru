using lib.StateImpl;

namespace lib.Structures
{
    public class GameplayOut : Move
    {
        public GameplayOut()
        {
        }

        public GameplayOut(Move move, State state)
        {
            claim = move.claim;
            pass = move.pass;
            this.state = state;
        }

        public State state;
    }
}