using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lib.Bittorrent.Bencoding
{
    public class BenDictionary : BenValue
    {
        public IDictionary<BenByteString, BenValue> Values { get; protected set; }

        public BenDictionary(Dictionary<BenByteString, BenValue> values)
        {
            Values = new SortedDictionary<BenByteString, BenValue>(values);
        }

        public T Get<T>(string key) where T : BenValue
        {
            Values.TryGetValue(new BenByteString(key), out BenValue benValue);
            return (benValue as T);
        }

        public bool HasString(string key)
        {
            return (Get<BenByteString>(key) is BenByteString);
        }

        public bool HasInt(string key)
        {
            return (Get<BenInteger>(key) is BenInteger);
        }

        public bool HasList(string key)
        {
            return (Get<BenList>(key) is BenList);
        }

        public bool HasDict(string key)
        {
            return (Get<BenDictionary>(key) is BenDictionary);
        }

        public string GetString(string key)
        {
            return Get<BenByteString>(key)?.AsString;
        }

        public byte[] GetStringBytes(string key)
        {
            return Get<BenByteString>(key)?.Value;
        }

        public long? GetInt(string key)
        {
            return Get<BenInteger>(key)?.Value;
        }

        public BenList GetList(string key)
        {
            return Get<BenList>(key);
        }

        public BenDictionary GetDict(string key)
        {
            return Get<BenDictionary>(key);
        }

        #region Shortcuts

        public List<string> GetListOfStrings(string key)
        {
            return (Get<BenList>(key) is BenList list)
                ? list.OfType<BenByteString>().Select(s => s.AsString).ToList()
                : new List<string>();
        }

        public List<BenDictionary> GetListOfDicts(string key)
        {
            return (Get<BenList>(key) is BenList list)
                ? list.OfType<BenDictionary>()
                : new List<BenDictionary>();
        }

        #endregion

        public override void Encode(Stream stream)
        {
            stream.WriteByte((byte)'d');

            foreach (var kvp in Values)
            {
                kvp.Key.Encode(stream);
                kvp.Value.Encode(stream);
            }

            stream.WriteByte((byte)'e');
        }
    }
}
