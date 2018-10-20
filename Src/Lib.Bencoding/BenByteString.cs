using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Lib.Bencoding
{
    public class BenByteString : BenValue, IEquatable<BenByteString>, IComparable<BenByteString>
    {
        public byte[] Value { get; protected set; }
        public string AsString { get; protected set; }

        public BenByteString(byte[] value)
        {
            Value = value;
            AsString = Encoding.UTF8.GetString(Value);
        }

        public BenByteString(string value)
            : this(Encoding.UTF8.GetBytes(value))
        {
        }

        public override string ToString()
        {
            return AsString;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BenByteString);
        }

        public bool Equals(BenByteString other)
        {
            return (other != null
                && Value.SequenceEqual(other.Value));
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (byte b in Value)
            {
                hashCode += b;
            }
            return hashCode;
        }

        public int CompareTo(BenByteString other)
        {
            return AsString.CompareTo(other.AsString);
        }

        public override void Encode(Stream stream)
        {
            foreach (char c in Value.Length.ToString())
            {
                stream.WriteByte((byte)c);
            }
            stream.WriteByte((byte)':');
            stream.Write(Value, 0, Value.Length);
        }
    }
}
