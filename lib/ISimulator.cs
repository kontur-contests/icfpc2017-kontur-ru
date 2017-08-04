using System.Collections.Generic;

namespace lib
{
    public interface ISimulator
    {
        void StartGame(List<IAi> gamers);
        GameState NextMove();
    }
}