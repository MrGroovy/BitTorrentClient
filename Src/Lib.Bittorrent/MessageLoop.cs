using Lib.Bittorrent.Swarm;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Bittorrent
{
    public class MessageLoop : IMessageLoop
    {
        private MessageFactory messageFactory;
        private ActionBlock<Message> inbox;

        public MessageLoop(MessageFactory messageFactory)
        {
            this.messageFactory = messageFactory;
            this.inbox = new ActionBlock<Message>(
                async m => await HandleMessage(m),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = -1 });            
        }

        public void PostReadMetaInfoFromFileMessage(string filePath) =>
            inbox.Post(
                messageFactory.CreateReadMetaInfoFromFileMessage(
                    filePath));

        public void PostCallTrackerMessage() =>
            inbox.Post(
                messageFactory.CreateCallTrackerMessage());

        public void PostConnectToPeerMessage(IPAddress ip, int port, byte[] peerId) =>
            inbox.Post(
                messageFactory.CreateConnectToPeerMessage(
                    ip,
                    port,
                    peerId));

        public void PostHandshakeReceivedMessage(IPAddress ip, int port, HandshakeMessage handshake) =>
            inbox.Post(
                messageFactory.CreateHandshakeReceivedMessage(
                    ip,
                    port,
                    handshake));

        public void PostKeepAliveReceivedMessage(IPAddress ip, int port) =>
            inbox.Post(
                messageFactory.CreateKeepAliveReceivedMessage(
                    ip,
                    port));

        public void PostReceiveErrorMessage(IPAddress ip, int port) =>
            inbox.Post(
                messageFactory.CreateReceiveErrorMessage(
                    ip,
                    port));

        public void PostDecideWhatToDoMessage() =>
            inbox.Post(
                messageFactory.CreateDecideWhatToDoMessage());

        private async Task HandleMessage(Message message)
        {
            await message.Execute(this);
        }
    }
}
