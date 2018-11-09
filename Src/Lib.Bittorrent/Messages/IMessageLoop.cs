using Lib.Bittorrent.Swarm;
using System.Net;

namespace Lib.Bittorrent.Messages
{
    public interface IMessageLoop
    {
        void PostReadMetaInfoFromFileCommand(string filePath);
        void PostCallTrackerCommand();
        void PostDecideWhatToDoCommand();
        void PostConnectToPeerCommand(IPAddress ip, int port, byte[] peerId);

        void PostHandshakeReceivedEvent(IPAddress ip, int port, HandshakeMessage handshake);
        void PostHaveReceivedEvent(IPAddress ip, int port, HaveMessage have);
        void PostBitfieldReceivedEvent(IPAddress ip, int port, BitfieldMessage bitfield);
        void PostKeepAliveReceivedEvent(IPAddress ip, int port);
        void PostReceiveErrorEvent(IPAddress ip, int port);
        void PostChokeReceivedEvent(IPAddress loopback, int v, ChokeMessage choke);
    }
}
