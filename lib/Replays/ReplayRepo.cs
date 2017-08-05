using System.Collections.Generic;

namespace lib.Replays
{
    public interface IReplayRepo
    {
        void SaveReplay(ReplayMeta meta, ReplayData data);

        List<ReplayMeta> GetRecentMetas(int limit = 10);
        ReplayData GetData(string dataId);
    }

    public class ReplayRepo : IReplayRepo
    {
        public void SaveReplay(ReplayMeta meta, ReplayData data)
        {
            throw new System.NotImplementedException();
        }

        public List<ReplayMeta> GetRecentMetas(int limit = 10)
        {
            throw new System.NotImplementedException();
        }

        public ReplayData GetData(string dataId)
        {
            throw new System.NotImplementedException();
        }
    }
}