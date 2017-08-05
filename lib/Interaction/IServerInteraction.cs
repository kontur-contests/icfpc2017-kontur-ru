using System;
using lib.Replays;

namespace lib.Interaction
{
    public interface IServerInteraction
    {
        void Start();
        Tuple<ReplayMeta, ReplayData> RunGame(IAi ai);
    }
}