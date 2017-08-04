using System.Drawing;
using lib;

namespace CinemaLib
{
    public class RiverPainterData
    {
        public float PenWidth;
        public Color Color;
        public string HoverText;
    }
    public class SitePainterData
    {
        public float Radius;
        public Color Color;
        public string HoverText;
    }

    public interface IPainterAugentor
    {
        SitePainterData GetData(Site site);
        RiverPainterData GetData(River river);
    }

}