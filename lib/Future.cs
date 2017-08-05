using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace lib
{
    public class Future
    {
        [JsonProperty("source")] public int Source;

        [JsonProperty("target")] public int Target;

        public Future()
        {
        }

        public Future(int source, int target)
        {
            Source = source;
            Target = target;
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}";
        }

        public static Future DecodeFrom(BinaryReader reader)
        {
            byte marker = reader.ReadByte();
            if (marker == 0) return null;
            var source = reader.ReadInt32();
            var target = reader.ReadInt32();
            return new Future(source, target);
        }

        public void EncodeTo(BinaryWriter writer)
        {
            writer.Write((byte)1);//marker
            writer.Write(Source);
            writer.Write(Target);
        }
    }
}