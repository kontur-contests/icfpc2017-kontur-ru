using System;

namespace lib.Interaction
{
    public interface IServerInteraction
    {
        void Start(string name, GameState state);

        event Action<Setup> SetupRecieved;
        Func<Move[], GameState, Tuple<Move, GameState>> HandleMove { set; }
        event Action<Move[], Score[], GameState> GameEnded;
    }
}