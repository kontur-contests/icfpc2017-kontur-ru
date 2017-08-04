namespace lib.viz.Detalization
{
    public interface IPainterAugmentor
    {
        IndexedMap Map { get; set; }
        SitePainterData GetData(Site site);
        RiverPainterData GetData(River river);
    }
}