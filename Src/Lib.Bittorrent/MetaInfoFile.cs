using System.Collections.Generic;

namespace Lib.Bittorrent
{
    public class MetaInfoFile
    {
        public List<string> Path { get; }
        public long Length { get; }

        public MetaInfoFile(List<string> path, long length)
        {
            Path = new List<string>(path);
            Length = length;
        }
    }
}
