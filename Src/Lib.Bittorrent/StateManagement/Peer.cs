using System.Net;

namespace Lib.Bittorrent.StateManagement
{
    public class Peer
    {
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }
        public byte[] PeerId { get; private set; }
        public bool[] PiecesAvailable { get; private set; }

        public PeerState State { get; set; }
        public bool IsInSwarm => State == PeerState.Connecting || State == PeerState.Connected;

        public Peer(IPAddress ip, int port, byte[] peerId, int numPieces)
        {
            Ip = ip;
            Port = port;
            PeerId = peerId;
            State = PeerState.Disconnected;
            PiecesAvailable = new bool[numPieces];
        }

        public void MarkPieceAsAvailable(int pieceIndex)
        {
            PiecesAvailable[pieceIndex] = true;
        }
    }
}
