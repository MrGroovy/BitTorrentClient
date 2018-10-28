using System.Linq;

namespace Lib.Bittorrent.Swarm
{
    public class BitfieldMessage : ProtocolMessage
    {
        public bool[] Bits { get; private set; }

        public BitfieldMessage(byte[] bitFieldBytes)
        {
            Bits = bitFieldBytes
                .SelectMany(ByteToBits)
                .ToArray();
        }

        private bool[] ByteToBits(byte bitsAsByte)
        {
            bool[] bitsAsBools = Enumerable
                .Range(0, 8)
                .Select(i => ((bitsAsByte << i) & 128) == 128)
                .ToArray();
            return bitsAsBools;
        }
    }
}
