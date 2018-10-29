using Microsoft.Extensions.Logging;

namespace Lib.Bittorrent.Swarm
{
    public class PeerClientFactory : IPeerClientFactory
    {
        private LoggerFactory logFactory;

        public PeerClientFactory(LoggerFactory logFactory)
        {
            this.logFactory = logFactory;
        }

        public IPeerClient Create() =>
            new PeerClient(
                new SystemSocket(),
                logFactory.CreateLogger<PeerClient>());
    }
}
