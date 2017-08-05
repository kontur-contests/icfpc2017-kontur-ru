using lib.Replays;

namespace lib.viz
{
    public class ReplayFullData
    {
        public ReplayFullData(ReplayMeta meta, ReplayData data)
        {
            Meta = meta;
            Data = data;
        }

        public ReplayMeta Meta;
        public ReplayData Data;
    }
}