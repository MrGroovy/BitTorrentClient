namespace Lib.Bittorrent
{
    public class Message
    {
    }

    public class KeepAlive : Message
    {
        public static readonly KeepAlive Instance = new KeepAlive();
    }

    public class Choke : Message
    {
        public static readonly Choke Instance = new Choke();
    }

    public class Unchoke : Message
    {
        public static readonly Unchoke Instance = new Unchoke();
    }

    public class Interested : Message
    {
        public static readonly Interested Instance = new Interested();
    }

    public class NotInterested : Message
    {
        public static readonly NotInterested Instance = new NotInterested();
    }

    public class Handshake : Message
    {
        public byte[] ProtocolString { get; private set; }
        public byte[] Reserved { get; private set; }
        public byte[] InfoHash { get; private set; }
        public byte[] PeerId { get; private set; }

        public Handshake(byte[] protocolString, byte[] reserved, byte[] infoHash, byte[] peerId)
        {
            ProtocolString = protocolString;
            Reserved = reserved;
            InfoHash = infoHash;
            PeerId = peerId;
        }
    }

    public class Have : Message
    {
        public int PieceIndex { get; private set; }

        public Have(int pieceIndex)
        {
            PieceIndex = pieceIndex;
        }
    }

    /*
    bool[] ByteToBits(byte bitsAsByte)
    {
        bool[] bitsAsBools = Enumerable
            .Range(0, 8)
            .Select(i => ((bitsAsByte << i) & 128) == 128)
            .ToArray();
        return bitsAsBools;
    }

    void ThrowIfAnyPaddingBitsAreSet(bool[] bits)
    {
        bool anyPaddingBitsSet = bits
            .Skip(MetaInfo.NumPieces)
            .Any(b => b);

        if (anyPaddingBitsSet)
            throw new InvalidOperationException("Padding bits set, closing peer.");
    }
    */
    public class Bitfield : Message
    {
        public byte[] Bytes { get; private set; }

        public Bitfield(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
