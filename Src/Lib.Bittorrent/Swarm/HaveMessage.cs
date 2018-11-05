namespace Lib.Bittorrent.Swarm
{
    public class HaveMessage : ProtocolMessage
    {
        public int PieceIndex { get; }

        public HaveMessage(int pieceIndex)
        {
            PieceIndex = pieceIndex;
        }
    }
}
