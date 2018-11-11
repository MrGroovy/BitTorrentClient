using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.StateManagement
{
    public interface ITorrentState
    {
        int NumberOfPieces { get; }
        void MarkPieceAsAvailable(IPAddress ip, int port, int pieceIndex);
        void MarkPiecesAsAvailable(IPAddress ip, int port, bool[] pieceIndexes);

        void SetIsHeChoking(IPAddress ip, int port, bool isHeChoking);

        Task RunInLock(Action action);
        Task RunInLock(Func<Task> action);
    }
}
