namespace lib.viz.Detalization
{
    public interface IPainterAugmentor
    {
        SitePainterData GetData(Site site);
        RiverPainterData GetData(River river);
    }
}