using System.Collections.Generic;
using lib.Ai;

namespace lib
{
    public interface ISimulator
    {
        void StartGame(List<IAi> gamers);
        GameState NextMove();
    }
}