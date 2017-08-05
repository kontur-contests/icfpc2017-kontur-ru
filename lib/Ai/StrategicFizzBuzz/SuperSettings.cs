using lib.Structures;

namespace lib.Ai.StrategicFizzBuzz
{
    public class SuperSettings
    {
        public SuperSettings(int punterId, int puntersCount, Map map, Settings settings)
        {
            PunterId = punterId;
            PuntersCount = puntersCount;
            Map = map;
            Settings = settings;
        }

        public int PunterId { get; }
        public int PuntersCount { get; }
        public Map Map { get; }
        public Settings Settings { get; }
    }
}