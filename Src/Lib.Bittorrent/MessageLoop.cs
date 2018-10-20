using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Bittorrent
{
    public class MessageLoop
    {
        private ActionBlock<Message> inbox;

        public MessageLoop()
        {
            inbox = new ActionBlock<Message>(
                async m => await HandleMessage(m),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = -1 });
        }

        public void Post(Message message)
        {
            inbox.Post(message);
        }

        private async Task HandleMessage(Message message)
        {
            await message.Execute(this);
        }
    }
}
