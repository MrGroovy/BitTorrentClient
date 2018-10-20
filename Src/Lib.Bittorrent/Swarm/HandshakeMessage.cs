using System;

namespace Lib.Bittorrent.Swarm
{
    public class HandshakeMessage
    {
        public string ProtocolString { get; private set; }
        public byte[] Reserved { get; private set; }
        public byte[] InfoHash { get; private set; }
        public byte[] PeerId { get; private set; }

        public HandshakeMessage(string protocolString, byte[] reserved, byte[] infoHash, byte[] peerId)
        {
            if (reserved.Length != 8) throw new ArgumentException("'Reserved' should be 8 bytes long.");
            if (infoHash.Length != 20) throw new ArgumentException("'infoHash' should be 20 bytes long.");
            if (peerId.Length != 20) throw new ArgumentException("'peerId' should be 20 bytes long.");

            ProtocolString = protocolString;
            Reserved = reserved;
            InfoHash = infoHash;
            PeerId = peerId;
        }
    }
}
