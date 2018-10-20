using System.IO;

namespace Lib.Bencoding
{
    public class BenInteger : BenValue
    {
        public long Value { get; protected set; }

        public BenInteger(long value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override void Encode(Stream stream)
        {
            stream.WriteByte((byte)'i');
            foreach (char c in Value.ToString())
            {
                stream.WriteByte((byte)c);
            }
            stream.WriteByte((byte)'e');
        }
    }
}
