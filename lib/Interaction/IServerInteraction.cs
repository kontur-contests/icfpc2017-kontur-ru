using System;

namespace lib
{
    public interface IServerInteraction
    {
        void Start(string name, GameState state);

        event Action<Setup> SetupRecieved;
        Func<AbstractMove[], GameState, Tuple<AbstractMove, GameState>> HandleMove { set; }
        event Action<AbstractMove[], Score[], GameState> GameEnded;
    }
}