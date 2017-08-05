using System.Collections.Generic;

namespace lib.Replays
{
    public interface IReplayRepo
    {
        void SaveReplay(ReplayMeta meta, ReplayData data);

        List<ReplayMeta> GetRecentMetas(int limit = 10);
        ReplayData GetData(string dataId);
    }
}