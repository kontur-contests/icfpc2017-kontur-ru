using lib.Structures;

namespace lib.viz.Detalization
{
    public interface IPainterAugmentor
    {
        IndexedMap Map { get; set; }
        SitePainterData GetData(Site site);
        RiverPainterData GetData(River river);
        FuturePainterData GetData(int punderId, Future future);
        bool ShowFutures { get; set; }
        int SelectedPlayerIndex { get; set; }
    }
}