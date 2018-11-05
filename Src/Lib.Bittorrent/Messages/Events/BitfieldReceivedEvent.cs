using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class BitfieldReceivedEvent : Message
    {
        private readonly IPAddress ip;
        private readonly int port;
        private readonly BitfieldMessage bitfield;
        private readonly ITorrentState state;
        private readonly ILogger<BitfieldReceivedEvent> log;

        public BitfieldReceivedEvent(IPAddress ip, int port, BitfieldMessage bitfield, ITorrentState state, ILogger<BitfieldReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.bitfield = bitfield;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            LogBitFieldReceived();

            try
            {
                ThrowIfAnyPaddingBitsAreSet();
                state.RunInLock(() => state.MarkPiecesAsAvailable(ip, port, bitfield.Bits));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Peer will be closed.");
                loop.PostReceiveErrorEvent(ip, port);
            }

            return Task.CompletedTask;
        }

        private void ThrowIfAnyPaddingBitsAreSet()
        {
            bool anyPaddingBitsSet = bitfield.Bits
                .Skip(state.NumberOfPieces)
                .Any(b => b);

            if (anyPaddingBitsSet)
                throw new InvalidOperationException("Padding bits set, closing peer.");
        }

        private void LogBitFieldReceived() =>
            log.LogTrace("Bitfield received from {ip}:{port}.",
                ip,
                port);
    }
}
