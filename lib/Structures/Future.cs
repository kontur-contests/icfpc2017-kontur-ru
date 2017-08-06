using System.IO;

namespace lib.Structures
{
    public class Future
    {
        public int source;
        public int target;

        public Future()
        {
        }

        public Future(int source, int target)
        {
            this.source = source;
            this.target = target;
        }

        public override string ToString()
        {
            return $"{nameof(source)}: {source}, {nameof(target)}: {target}";
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
            writer.Write((int) source);
            writer.Write((int) target);
        }
    }
}