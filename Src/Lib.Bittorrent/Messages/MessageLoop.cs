using Lib.Bittorrent.Swarm;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Bittorrent.Messages
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

        private async Task HandleMessage(Message message)
        {
            await message.Execute(this);
        }

        public void PostReadMetaInfoFromFileCommand(string filePath) =>
            inbox.Post(
                messageFactory.CreateReadMetaInfoFromFileCommand(
                    filePath));

        public void PostCallTrackerCommand() =>
            inbox.Post(
                messageFactory.CreateCallTrackerCommand());

        public void PostDecideWhatToDoCommand() =>
            inbox.Post(
                messageFactory.CreateDecideWhatToDoCommand());

        public void PostConnectToPeerCommand(IPAddress ip, int port, byte[] peerId) =>
            inbox.Post(
                messageFactory.CreateConnectToPeerCommand(
                    ip,
                    port,
                    peerId));

        public void PostHandshakeReceivedEvent(IPAddress ip, int port, HandshakeMessage handshake) =>
            inbox.Post(
                messageFactory.CreateHandshakeReceivedEvent(
                    ip,
                    port,
                    handshake));

        public void PostChokeReceivedEvent(IPAddress ip, int port, ChokeMessage choke) =>
            inbox.Post(
                messageFactory.CreateChokeReceivedEvent(
                    ip,
                    port,
                    choke));

        public void PostUnchokeReceivedEvent(IPAddress ip, int port, UnchokeMessage unchoke) =>
            inbox.Post(
                messageFactory.CreateUnchokeReceivedEvent(
                    ip,
                    port,
                    unchoke));

        public void PostHaveReceivedEvent(IPAddress ip, int port, HaveMessage have) =>
            inbox.Post(
                messageFactory.CreateHaveReceivedEvent(
                    ip,
                    port,
                    have));

        public void PostBitfieldReceivedEvent(IPAddress ip, int port, BitfieldMessage bitfield) =>
            inbox.Post(
                messageFactory.CreateBitfieldReceivedEvent(
                    ip,
                    port,
                    bitfield));

        public void PostKeepAliveReceivedEvent(IPAddress ip, int port) =>
            inbox.Post(
                messageFactory.CreateKeepAliveReceivedEvent(
                    ip,
                    port));

        public void PostReceiveErrorEvent(IPAddress ip, int port) =>
            inbox.Post(
                messageFactory.CreateReceiveErrorEvent(
                    ip,
                    port));
    }
}
