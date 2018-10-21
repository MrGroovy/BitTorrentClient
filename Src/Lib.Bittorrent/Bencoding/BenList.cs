using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lib.Bittorrent.Bencoding
{
    public class BenList : BenValue
    {
        public List<BenValue> Values { get; }

        public BenList(IEnumerable<BenValue> values)
        {
            Values = new List<BenValue>(values);
        }

        public override void Encode(Stream stream)
        {
            stream.WriteByte((byte)'l');
            foreach (BenValue value in Values)
            {
                value.Encode(stream);
            }
            stream.WriteByte((byte)'e');
        }

        public List<T> OfType<T>() where T : BenValue
        {
            return Values.OfType<T>().ToList();
        }
    }
}
