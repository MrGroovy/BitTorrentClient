using System.IO;

namespace Lib.Bittorrent.Bencoding
{
    public abstract class BenValue
    {
        public abstract void Encode(Stream stream);
    }
}
