using System.IO;

namespace Lib.Bencoding
{
    public abstract class BenValue
    {
        public abstract void Encode(Stream stream);
    }
}
