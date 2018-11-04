using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.StateManagement
{
    public interface ITorrentState
    {
        int NumberOfPieces { get; }
        void MarkPieceAsAvailable(IPAddress ip, int port, int pieceIndex);

        Task RunInLock(Action action);
        Task RunInLock(Func<Task> action);
    }
}
