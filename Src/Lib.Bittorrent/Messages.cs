using System;
using System.Linq;

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

        public Have(int pieceIndex, MetaInfo metaInfo)
        {
            AssertCorrectIndex(pieceIndex, metaInfo);

            PieceIndex = pieceIndex;
        }

        private void AssertCorrectIndex(int pieceIndex, MetaInfo metaInfo)
        {
            bool indexCorrect = pieceIndex >= 0 && pieceIndex <= metaInfo.NumPieces - 1;

            if (indexCorrect is false)
                throw new ArgumentException($"Have message has invalid index {pieceIndex}.");
        }
    }

    public class Bitfield : Message
    {
        public bool[] Bits { get; private set; }

        public Bitfield(byte[] bytes, MetaInfo metaInfo)
        {
            AssertMessageLength(bytes, metaInfo);
            AssertNoPaddingBitsOn(bytes, metaInfo);

            Bits = bytes
                .SelectMany(b => ByteToBits(b))
                .Take(metaInfo.NumPieces)
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

        private void AssertMessageLength(byte[] bytes, MetaInfo metaInfo)
        {
            int minBits = metaInfo.NumPieces;
            int maxBits = metaInfo.NumPieces + 8;
            int gotBits = bytes.Length * 8;
            bool lengthOk = gotBits >= minBits && gotBits <= maxBits;

            if (lengthOk is false)
                throw new ArgumentException("Bitfield should have correct length.");
        }

        private void AssertNoPaddingBitsOn(byte[] bytes, MetaInfo metaInfo)
        {
            bool noPadding = bytes
                .SelectMany(b => ByteToBits(b))
                .Skip(metaInfo.NumPieces)
                .All(b => b is false);

            if (noPadding is false)
                throw new ArgumentException("Bitfield should not have padding bits set.");
        }
    }
}
