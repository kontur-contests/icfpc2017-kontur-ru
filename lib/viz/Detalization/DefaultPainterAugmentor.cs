using System.Drawing;

namespace lib.viz.Detalization
{
    public class DefaultPainterAugmentor : IPainterAugmentor
    {
        private IndexedMap map;

        public Map Map
        {
            get => map.Map;
            set => map = new IndexedMap(value);
        }

        public SitePainterData GetData(Site site)
        {
            return new SitePainterData
            {
                Color = GetSiteColor(site),
                Radius = 3
            };
        }

        public RiverPainterData GetData(River river)
        {
            return new RiverPainterData
            {

                Color = river.Owner == -1 ? Color.Blue : Color.FromKnownColor((KnownColor) river.Owner) ,
                PenWidth = 1,
            };
        }

        private Color GetSiteColor(Site site)
        {
            return map.MineIds.Contains(site.Id) ? Color.Red : Color.LimeGreen;
        }
    }
}