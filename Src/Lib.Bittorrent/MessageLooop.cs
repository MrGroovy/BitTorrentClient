using Lib.Bittorrent.Swarm;
using System.Net;

namespace Lib.Bittorrent
{
    public interface IMessageLooop
    {
        void PostPeerConnectedMessage(IPAddress ip, int port);
        void PostHandshakeReceivedMessage(IPAddress ip, int port, HandshakeMessage handshake);
        void PostKeepAliveReceivedMessage(IPAddress ip, int port);
    }

    public class MessageLooop : IMessageLooop
    {
        private readonly MessageFactory msgFactory;
        private readonly MessageLoop msgLoop;

        public MessageLooop(MessageFactory msgFactory, MessageLoop msgLoop)
        {
            this.msgFactory = msgFactory;
            this.msgLoop = msgLoop;
        }

        public void PostPeerConnectedMessage(IPAddress ip, int port) =>
            msgLoop.Post(
                msgFactory.CreateConnectedToPeerMessage(
                    ip,
                    port));

        public void PostHandshakeReceivedMessage(IPAddress ip, int port, HandshakeMessage handshake) =>
            msgLoop.Post(
                msgFactory.CreateHandshakeReceivedMessage(
                    ip,
                    port,
                    handshake));

        public void PostKeepAliveReceivedMessage(IPAddress ip, int port) =>
            msgLoop.Post(
                msgFactory.CreateKeepAliveReceivedMessage(
                    ip,
                    port));
    }
}
