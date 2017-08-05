using System;
using lib.Ai;
using lib.Replays;

namespace lib.Interaction
{
    public interface IServerInteraction
    {
        bool Start();
        Tuple<ReplayMeta, ReplayData> RunGame(IAi ai);
    }
}